using HomeBankingMindHub.dtos;

using HomeBankingMindHub.Models;

using HomeBankingMindHub.Repositories;

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Net;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();

                var AccountsDTO = new List<AccountDTO>();

                foreach (Account account in accounts)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(tr => new TransactionDTO
                        {
                            Id = tr.Id,

                            Type = tr.Type,

                            Amount = tr.Amount,

                            Description = tr.Description,

                            Date = tr.Date,
                        }).ToList()
                    };

                    AccountsDTO.Add(newAccountDTO);

                }

                return Ok(AccountsDTO);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var account = _accountRepository.FindById(id);

                if (account == null)
                {
                    return NotFound();
                }

                var accountDTO = new AccountDTO
                {
                    Id = account.Id,

                    Number = account.Number,

                    CreationDate = account.CreationDate,

                    Balance = account.Balance,

                    Transactions = account.Transactions.Select(tr => new TransactionDTO
                    {
                        Id = tr.Id,

                        Type = tr.Type,

                        Amount = tr.Amount,

                        Description = tr.Description,

                        Date = tr.Date,
                    }).ToList()
                };

                return Ok(accountDTO);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost()]
        public AccountDTO Post(long clientId)
        {
            try
            {
                Random random = new Random();

                int numeroAleatorio = random.Next(100000, 999999); //TENGO QUE VALIDAR QUE EL NUMERO DE TARJETA NO ESTE EN USO

                Account newAccount = new Account
                {
                    ClientId = clientId,
                    CreationDate = DateTime.Now,
                    Number = "VIN-" + numeroAleatorio.ToString(), //new Random().Next(100000,999999).ToString()
                    Balance = 0,
                };

                _accountRepository.Save(newAccount);
                AccountDTO accountDTO = new AccountDTO
                {
                    Id = newAccount.Id,
                    Balance = newAccount.Balance,
                    CreationDate = newAccount.CreationDate,
                    Number = newAccount.Number
                };
                return accountDTO; //TIENE QUE DEVOLVER UN DTO! (SOLO PARA EL METODO GET)
            }

            catch
            {
                return null;
            }
        }


        [HttpGet()]
        public IActionResult GetByClient(long clientId)
        {
            try
            {
                var accounts = _accountRepository.GetAccountsByClient(clientId);

                if(accounts == null) return Forbid();

                var accountsDTO = new List<AccountDTO>();

                foreach (Account account in accounts)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(tr => new TransactionDTO
                        {
                            Id = tr.Id,

                            Type = tr.Type,

                            Amount = tr.Amount,

                            Description = tr.Description,

                            Date = tr.Date,
                        }).ToList()
                    };

                    accountsDTO.Add(newAccountDTO);
                }                

                return Ok(accountsDTO);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
