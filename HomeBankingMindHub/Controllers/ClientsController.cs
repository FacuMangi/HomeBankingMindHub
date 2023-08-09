using HomeBankingMindHub.dtos;

using HomeBankingMindHub.Models;

using HomeBankingMindHub.Repositories;

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Net;
using System.Security.Principal;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;

        private AccountsController _accountsController; //Estaba mal usar la interfaz de accounts, para no romper con el patron de repostorio puedo traerme el controller de account directamente, cuya dependencia fue inyectada en Startup

        public ClientsController(IClientRepository clientRepository, AccountsController accountsController)

        {

            _clientRepository = clientRepository;

            _accountsController = accountsController;

        }

        [HttpGet]
        public IActionResult Get()

        {

            try

            {

                var clients = _clientRepository.GetAllClients();



                var clientsDTO = new List<ClientDTO>();



                foreach (Client client in clients)

                {

                    var newClientDTO = new ClientDTO

                    {

                        Id = client.Id,

                        Email = client.Email,

                        FirstName = client.FirstName,

                        LastName = client.LastName,

                        Accounts = client.Accounts.Select(ac => new AccountDTO

                        {

                            Id = ac.Id,

                            Balance = ac.Balance,

                            CreationDate = ac.CreationDate,

                            Number = ac.Number

                        }).ToList(),
                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,

                            LoanId = cl.LoanId,

                            Name = cl.Loan.Name,

                            Amount = cl.Amount,

                            Payments = int.Parse(cl.Payments)

                        }).ToList(),
                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,

                            CardHolder = c.CardHolder,

                            Color = c.Color,

                            Cvv = c.Cvv,

                            FromDate = c.FromDate,

                            Number = c.Number,

                            ThruDate = c.ThruDate,

                            Type = c.Type

                        }).ToList()

                    };



                    clientsDTO.Add(newClientDTO);

                }





                return Ok(clientsDTO);

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

                var client = _clientRepository.FindById(id);

                if (client == null)

                {

                    return NotFound();

                }



                var clientDTO = new ClientDTO

                {

                    Id = client.Id,

                    Email = client.Email,

                    FirstName = client.FirstName,

                    LastName = client.LastName,

                    Accounts = client.Accounts.Select(ac => new AccountDTO

                    {

                        Id = ac.Id,

                        Balance = ac.Balance,

                        CreationDate = ac.CreationDate,

                        Number = ac.Number

                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,

                        LoanId = cl.LoanId,

                        Name = cl.Loan.Name,

                        Amount = cl.Amount,

                        Payments = int.Parse(cl.Payments)

                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()

                };



                return Ok(clientDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }


        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {//recibo las cookies, corrovoro que el user tenga el "Client" y si su email no es vacio
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

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost()]
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");

                //buscamos si ya existe el usuario
                Client user = _clientRepository.FindByEmail(client.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);

                _accountsController.Post(newClient.Id); //Este es el metodo que cree en AccountsController para crear cuentas, lo llamo con el controlador

                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("current/accounts")]
        public IActionResult PostNewAccount()
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

                if (client.Accounts.Count > 2) 
                {
                    return StatusCode(403, "No puedes tener mas de 3 cuentas :)");
                }

                var newClientAccount = _accountsController.Post(client.Id);

                if (newClientAccount == null) 
                {
                    return StatusCode(500, "Error");
                }

                return Created("", newClientAccount);                
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
