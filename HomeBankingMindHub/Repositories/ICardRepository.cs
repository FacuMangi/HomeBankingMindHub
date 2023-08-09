using HomeBankingMindHub.Models;
using System.Collections.Generic;

namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCadrs();
        void Save(Card card);
        IEnumerable<Card> GetCardsByClient(long clientId);
    }
}
