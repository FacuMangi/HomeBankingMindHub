using HomeBankingMindHub.Controllers;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HomeBankingMindHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddControllers().AddJsonOptions(x =>

               x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
            //Agregando contexto de la base de datos
            services.AddDbContext<HomeBankingContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("HomeBankingConexion")));
            
            services.AddScoped<IClientRepository, ClientRepository>();

            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<ICardRepository, CardRepository>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddScoped<ILoanRepository, LoanRepository>();

            services.AddScoped<IClientLoanRepository, ClientLoanRepository>();

            services.AddScoped<AccountsController>();

            services.AddScoped<CardsController>();

            //autenticación
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.LoginPath = new PathString("/index.html");
            });

            //autorización
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            //le decimos que use autenticación
            app.UseAuthentication();
            //autorización

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
