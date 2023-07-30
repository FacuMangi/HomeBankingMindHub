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
                    foreach(Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();
                }
            }


            }
        }
    }
}
