using BL.Abstract;
using BL.DbHandling;
using Microsoft.Extensions.DependencyInjection;
using Models;
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
            serviceCollection.AddTransient<IDataHandler<Protocol>, ProtocolDataHandler>();
        }
    }
}
