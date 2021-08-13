using Microsoft.Extensions.DependencyInjection;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using Microsoft.Extensions.Configuration;
using BL;
using ArchiveApp.Services;
using ArchiveApp.Abstract;

namespace ArchiveApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ViewModelFactory>();

            serviceCollection.AddTransient(x =>
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appSettings.json");

                return builder.Build();
            });

            //serviceCollection.AddDbContext<AppContext>((pr, opts) =>
            //{

            //    //opts.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //}, ServiceLifetime.Transient);

            var factory = new AppContextFactory();

            serviceCollection.AddTransient<AppContext>(x =>
            {
                return factory.CreateDbContext(null);
            });

            serviceCollection.AddTransient<IDefaultItemsViewModel>(pr =>
            {
                var f = pr.GetService<ViewModelFactory>();
                return f.GetItemsViewModel(pr);
            });

            serviceCollection.AddTransient<XmlFileService>();
            serviceCollection.AddSingleton<DropDownDataService>();
        }
    }
}
