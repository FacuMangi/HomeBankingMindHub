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
        }
    }
}
