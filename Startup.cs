using System.IO;
using AzureFunctionsInProcess.Contracts;
using AzureFunctionsInProcess.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Shop
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
        }
    }

}