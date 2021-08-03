using BL.DbHandling;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<DbConnectionHandler>();
            serviceCollection.AddTransient<AppContextLoader>();
        }
    }
}
