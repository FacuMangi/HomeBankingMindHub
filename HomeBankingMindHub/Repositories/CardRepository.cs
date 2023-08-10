using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HomeBankingMindHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }
        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }

        public IEnumerable<Card> GetCardsByClient(long clientId)
        {
            return FindByCondition(card => card.ClientId == clientId);
        }

        public Card FindByNumber(string number) //metodo que me permite buscar una tarjeta por su numero en la base de datos
        {
            return FindByCondition(card => card.Number == number)
                .FirstOrDefault();
        }
    }
}
