using HomeBankingMindHub.Models;
using System.Collections;
using System.Collections.Generic;

namespace HomeBankingMindHub.Repositories
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();
        void Save(Transaction transaction);
        Transaction FindByNumber(long id);
    }
}
