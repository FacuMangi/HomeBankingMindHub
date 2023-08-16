using HomeBankingMindHub.Models;
using System.Collections.Generic;

namespace HomeBankingMindHub.Repositories
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAllLoans();

        Loan FindById(long id);
    }
}
