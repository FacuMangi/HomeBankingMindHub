using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Models
{
    public class HomeBankingContext: DbContext// DbContext se encarga del mapeo de los modelos en la base de datos
    {
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options): base(options) { } //constructor de la clase homebanking, toma como parametro DbContextOptions<HomeBankingContext> que se utilizan para configurar las opciones del contexto
        //luego llama al constructor base dbcontext (base) y le pasa las opciones

        //dbsets que representan las tablas en la base de datos
        public DbSet<Client> Clients { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<ClientLoan> ClientLoans { get; set; }

        public DbSet<Card> Cards { get; set; }
    }
}
