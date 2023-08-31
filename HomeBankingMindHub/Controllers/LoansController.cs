using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : Controller
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository,
            ITransactionRepository transactionRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
        }

        [HttpPost]
        //Con el post estoy enviando los datos de la solicitud del prestamo
        public IActionResult Post([FromBody] LoanApplicationDTO loanAppDto) 
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty; //preguntamos si el client es nulo o si está vacio, si es distinto de nulo nos fijamos si tiene un valor o si esta vacia la cadena
                if (email == string.Empty) //verificamos si el email esta vacio, si es así devuelve prohibido
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                //valido que la cuenta de destino exista
                var account = _accountRepository.FindByNumber(loanAppDto.ToAccountNumber);
                
                if (account == null)
                {
                    return StatusCode(403, "Fallo de validacion en solicitud de prestamo");
                }

                //valido que la cuenta de destino del prestamo pertenezca al Cliente autenticado que lo solicitó
                if (account.ClientId != client.Id) 
                {
                    return StatusCode(403, "Fallo de validacion en solicitud de prestamo");
                }

                //valido que el prestamo exista
                var loan = _loanRepository.FindById(loanAppDto.LoanId);
                
                if (loan == null) 
                {
                    return StatusCode(403, "Fallo de validacion en solicitud de prestamo");
                }

                //valido que la cantidad el prestamo no sea 0 y que no supere la cantidad máxima permitida
                if (loanAppDto.Amount == 0 || loanAppDto.Amount > loan.MaxAmount)
                {
                    return StatusCode(403, "Fallo de validacion en solicitud de prestamo");
                }

                //valido que los pagos no sean strings vacios o nulos
                if (loan.Payments == string.Empty || loan.Payments == null)
                {
                    return StatusCode(403, "Fallo de validacion en solicitud de prestamo");
                }

                //valido si el string payments de la solicitud del prestamo está dentro del string payments del modelo prestamo, de esa manera valido que la cantidad de cuotas se encuentre entre las disponibles del préstamo
                if (!loan.Payments.Contains(loanAppDto.Payments))
                {
                    return StatusCode(403, "Fallo de validacion en solicitud de prestamo");
                }

                //validacion de tipo de dato del dto (payment tiene que ser un string que pueda parsearse a un entero)
                if (!int.TryParse(loanAppDto.Payments, out int numericPayments))
                {
                    return StatusCode(403, "Fallo de validacion en solicitud de prestamo");
                }

                var porcent = loanAppDto.Amount * 20 / 100;

                var newClientLoan = new ClientLoan
                {
                    LoanId = loanAppDto.LoanId,
                    Amount = loanAppDto.Amount + porcent,
                    Payments = loanAppDto.Payments,
                    ClientId = client.Id,
                };

                var newClientLoanDTO = new ClientLoanDTO
                {
                    LoanId = newClientLoan.LoanId,
                    Amount = newClientLoan.Amount,
                    Payments = numericPayments,
                    Name = loan.Name,
                };

                _clientLoanRepository.Save(newClientLoan);

                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = loanAppDto.Amount,
                    Description = loan.Name + " " + "loan approved",
                    AccountId = account.Id,
                    Date = DateTime.Now,
                });

                //Actualizo la cuenta de destino sumando el monto solicitado
                account.Balance = account.Balance + loanAppDto.Amount;
                _accountRepository.Save(account);

                return Created("Solicitud de prestamo creada", newClientLoanDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();

                if (loans == null) return Forbid();

                var loansDTO = new List<LoanDTO>();

                foreach (Loan loan in loans)
                {
                    var newLoanDTO = new LoanDTO
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                    };
                    loansDTO.Add(newLoanDTO);
                }

                return Ok(loansDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
