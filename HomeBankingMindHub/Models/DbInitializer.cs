using System;
using System.Linq;

namespace HomeBankingMindHub.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client {Email = "datainicial@gmail.com", FirstName = "Cosme", LastName = "Fulanito", Password = "Password"}
                };

                foreach (Client client in clients)
                {
                    context.Clients.Add(client);
                }
                //guardando
                context.SaveChanges();
                
            }

            //else
            //{
            //    var clients = new Client[]
            //    {
            //        new Client {Email = "datadeprueba@gmail.com", FirstName = "Cosme", LastName = "Fulanito", Password = "Password"}
            //    };

            //    foreach (Client client in clients)
            //    {
            //        context.Clients.Add(client);
            //    }

            //    context.SaveChanges();
            //}

            if (!context.Accounts.Any())
            {
                var dummyClient = context.Clients.FirstOrDefault(c => c.Email ==
                "datainicial@gmail.com");
                if (dummyClient != null)
                {
                    var accounts = new Account[]
                    {
                        new Account{ClientId = dummyClient.Id, CreationDate = DateTime.Now, Number = string.Empty,
                        Balance = 500000}
                    };
                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();
                }
            }

            //else
            //{
            //    var dummyClient = context.Clients.FirstOrDefault(c => c.Email ==
            //    "datadeprueba@gmail.com");
            //    if (dummyClient != null)
            //    {
            //        var accounts = new Account[]
            //        {
            //            new Account{ClientId = dummyClient.Id, CreationDate = DateTime.Now, Number = "VIN001",
            //            Balance = 500000}
            //        };
            //        foreach (Account account in accounts)
            //        {
            //            context.Accounts.Add(account);
            //        }
            //        context.SaveChanges();
            //    }
            //}

            if (!context.Transactions.Any())

            {

                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");

                if (account1 != null)

                {

                    var transactions = new Transaction[]

                    {

                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia reccibida", Type = TransactionType.CREDIT.ToString() },

                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT.ToString() },

                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT.ToString() },

                    };

                    foreach (Transaction transaction in transactions)

                    {

                        context.Transactions.Add(transaction);

                    }

                    context.SaveChanges();



                }

            }



        }
    }
}
