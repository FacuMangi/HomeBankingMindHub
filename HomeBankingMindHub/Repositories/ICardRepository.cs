using HomeBankingMindHub.Models;
using System.Collections.Generic;

namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {
        void Save(Card card);
        IEnumerable<Card> GetCardsByClient(long clientId);
        Card FindByNumber(string number);
    }
}
