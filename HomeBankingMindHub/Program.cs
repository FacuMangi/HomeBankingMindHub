using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBankingMindHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
          //CreateHostBuilder(args).Build().Run();
          var host = CreateHostBuilder(args).Build(); //crea el host usando el constructor constructor y lo construye
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider; //obtiene el provedor de servicios del host
                try
                {
                    var context = services.GetRequiredService<HomeBankingContext>(); //obtiene el contexto de la base de datos
                    DbInitializer.Initialize(context);//inicializa la base de datos
                }
                catch(Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>(); //en caso de un error obtiene el servicio para registrar el error
                    logger.LogError(ex, "Ocurrió un error al enviar la información a la base de datos");
                }
            }
            host.Run(); //inicia la ejecucion del host
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
