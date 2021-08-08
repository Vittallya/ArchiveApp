using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Models
{
    class AppContextFactory : IDesignTimeDbContextFactory<AppContext>
    {
        public AppContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder();
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appSettings.json");

            var json = config.Build();

            var str = json.GetConnectionString("AltConnection1");
            var opt = new DbContextOptionsBuilder().UseSqlServer(str);



            return new AppContext(opt.Options);
        }
    }
}
