using Microsoft.Extensions.DependencyInjection;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using Microsoft.Extensions.Configuration;
using BL;

namespace ArchiveApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            
            serviceCollection.AddTransient(x =>
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appSettings.json");

                return builder.Build();
            });

            serviceCollection.AddDbContext<AppContext>((pr, opts) =>
            {
                var s = pr.GetService<DbConnectionHandler>();
                var str = s.ActualConnectionString;
                opts.UseSqlServer(str);
            });
        }
    }
}
