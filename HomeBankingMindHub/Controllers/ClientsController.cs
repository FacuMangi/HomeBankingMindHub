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

using System.Text.RegularExpressions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;

        private AccountsController _accountsController; //Estaba mal usar la interfaz de accounts, para no romper con el patron de repostorio puedo traerme el controller de account directamente, cuya dependencia fue inyectada en Startup

        private CardsController _cardsController;
        public ClientsController(IClientRepository clientRepository, AccountsController accountsController, CardsController cardsController)

        {
            _clientRepository = clientRepository;

            _accountsController = accountsController;

            _cardsController = cardsController;
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
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty; 
                if (email == string.Empty) 
                {
                    return StatusCode(403, "Debes ser cliente"); ;
                }
                //recibo las cookies, corrovoro que el user tenga el "Client" y si su email no es vacio
                //preguntamos si el client es nulo o si está vacio, si es distinto de nulo nos fijamos si tiene un valor o si esta vacia la cadena
                //verificamos si el email esta vacio, si es así devuelve prohibido

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return StatusCode(403, "Debes logearte para acceder a tus datos"); ;
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
                //validamos datos que no sean nulos o vacios
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");
                

                //validamos datos nombre y apellido tengan mas de 3 caracteres 
                static int CountCharacters(string chain)
                {
                    return chain.Length;
                }
                
                if (CountCharacters(client.FirstName) < 3 || CountCharacters(client.LastName) < 3)
                    return StatusCode(403, "Minimo 3 caracteres en nombre y apellido");


                //validamos nombre y apellido no tengan numeros ni caracteres especiales
                static bool ContainNumbersOrSpecial(string chain)
                {
                    // Verificar si contiene números
                    bool containNumbers = Regex.IsMatch(chain, @"\d");

                    // Verificar si contiene caracteres especiales (no letras ni números)
                    bool containSpecial = Regex.IsMatch(chain, @"^[a-zA-Z0-9]+$");

                    return containNumbers || containSpecial;
                }

                if (!ContainNumbersOrSpecial(client.FirstName) || !ContainNumbersOrSpecial(client.LastName))
                    return StatusCode(403, "No puedes poner numeros ni caracteres especiales en el nombre o el apellido");


                //validamos contraseña al menos 8 caracteres
                if (CountCharacters(client.Password) < 8)
                    return StatusCode(403, "Password debe tener mas de 8 caracteres");


                //validamos contraseña con una mayuscula, una minuscula y un numero
                static bool HasAny(string chain)
                {
                    bool anyNumber = chain.Any(char.IsDigit);
                    bool anyUpper = chain.Any(char.IsUpper);
                    bool anyLower = chain.Any(char.IsLower);

                    return anyNumber && anyUpper && anyLower;
                }

                if (!HasAny(client.Password))
                    return StatusCode(403, "Password debe contener un numero, una mayúscula y una minúscula");


                //validamos si el email es válido
                static bool IsValid(string email)
                {
                    string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";

                    bool isEmailValid = Regex.IsMatch(email, pattern);

                    return isEmailValid;
                }

                if (!IsValid(client.Email))
                    return StatusCode(403, "Email no valido");


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

                var newClientAccount = _accountsController.Post(client.Id); //De acá saca los parametros con los que llama al metodo, del client autenticado

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


        [HttpGet("current/accounts")]
        public IActionResult GetClientAccounts() 
        {
            try 
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty; 
                if (email == string.Empty) 
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                //var clientAccountsList = _accountsController.GetByClient(client.Id);

                //return Ok(clientAccountsList);
                //

                var accounts = client.Accounts;

                //if (accounts == null) return Forbid();

                var accountsDTO = new List<AccountDTO>();

                foreach (Account account in accounts)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

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


        [HttpPost("current/cards")]
        public IActionResult PostNewCard([FromBody] Card card)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty; 
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                if(String.IsNullOrEmpty(card.Type) || String.IsNullOrEmpty(card.Color)) 
                {
                    return StatusCode(403, "datos inválidos");
                }

                //Valido que se seleccione un tipo de tarjeta valido
                if (card.Type != "CREDIT" && card.Type != "DEBIT")
                    return StatusCode(403, "Tipo de tarjeta no valido");

                //Valido que se elija un color valido
                if (card.Color != "GOLD" && card.Color != "SILVER" && card.Color != "TITANIUM")
                    return StatusCode(403, "Color de tarjeta no valido");

                //No mas de 3 tarjetas de cada tipo
                var cardsAmount = 0;

                foreach (Card cardAux in client.Cards)
                {
                    if(cardAux.Type == card.Type)
                    {
                        cardsAmount++;
                    }
                }

                if(cardsAmount >= 3)
                {
                    return Forbid($"No puede crearse mas tarjetas del tipo {card.Type}");
                }

                var newClientCardDTO = _cardsController.Post(client.Id, client.FirstName, client.LastName, card.Type, card.Color);

                if (newClientCardDTO == null)
                {
                    return StatusCode(500, "Error");
                }

                return Created("", newClientCardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("current/cards")]
        public IActionResult GetClientCards()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                var clientCardsList = _cardsController.GetByClient(client.Id);

                return Ok(clientCardsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
