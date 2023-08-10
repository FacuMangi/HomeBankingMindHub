using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace HomeBankingMindHub.Controllers
{
    public class CardsController : ControllerBase
    {
        private ICardRepository _cardRepository;


        public CardsController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [HttpPost()]
        public CardDTO Post(long clientId, string FirstName, string LastName, string Type, string Color)
        {
            try
            {

                var digits = new Random().Next(1000, 9999);
                var digits2 = new Random().Next(1000, 9999);
                var digits3 = new Random().Next(1000, 9999);
                var digits4 = new Random().Next(1000, 9999);

                string newCardNumber;

                //ejecuto el bucle que genera numeros de tarjeta hasta que se genere un número que no esté presente en la base de datos de tarjetas
                do
                {
                    newCardNumber = digits.ToString() + "-" + digits2.ToString() + "-" + digits3.ToString() + "-" + digits4.ToString();
                }
                while (_cardRepository.FindByNumber(newCardNumber) != null);


                int yearsThru = 0;

                if (Color.ToUpper() == "SILVER")
                {
                    yearsThru = 3;
                }
                else if (Color.ToUpper() == "GOLD")
                {
                    yearsThru = 4;
                }
                else if (Color.ToUpper() == "TITANIUM")
                {
                    yearsThru = 5;
                }

                Card newCard = new Card
                {
                    ClientId = clientId,
                    CardHolder = FirstName + " " + LastName,
                    Type = Type,
                    Color = Color,
                    Number = newCardNumber,
                    Cvv = new Random().Next(100, 999),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(yearsThru),
                };

                _cardRepository.Save(newCard);

                CardDTO cardDTO = new CardDTO
                {
                    Id = newCard.Id,
                    CardHolder = newCard.CardHolder,
                    Type = newCard.Type,
                    Color = newCard.Color,
                    Number = newCard.Number,
                    Cvv = newCard.Cvv,
                    FromDate = newCard.FromDate,
                    ThruDate = newCard.ThruDate,
                };

                return cardDTO;

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
                var cards = _cardRepository.GetCardsByClient(clientId);

                var cardsDTO = new List<CardDTO>();

                foreach (Card card in cards)
                {
                    var newCardDTO = new CardDTO 
                    {
                        Id = card.Id,
                        CardHolder = card.CardHolder,
                        Color = card.Color,
                        Cvv = card.Cvv,
                        FromDate = card.FromDate,
                        Number = card.Number,
                        ThruDate = card.ThruDate,
                        Type = card.Type,
                    };

                    cardsDTO.Add(newCardDTO);
                }

                return Ok(cardsDTO);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }    

}
