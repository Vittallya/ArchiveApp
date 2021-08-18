using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AppContextFactory : IDesignTimeDbContextFactory<AppContext>
    {
        public const string FILE_NAME = "connection.txt";
        public static string ActualConnectionString { get; private set; }

        public AppContext CreateDbContext(string[] args)
        {
            var opt = new DbContextOptionsBuilder();
            opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            if (ActualConnectionString == null)
            {
                if (!FindConnection())
                {
                    throw new ArgumentException("Working connection was not found");
                }
            }

            opt.UseSqlServer(ActualConnectionString);            

            return new AppContext(opt.Options);
        }


        private bool FindConnection()
        {

            if (File.Exists(FILE_NAME))
            {
                string conn = File.ReadAllText(FILE_NAME);
                if (CheckConnection(conn))
                {
                    ActualConnectionString = conn;
                    return true;
                }
            }

            var config = new ConfigurationBuilder();
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appSettings.json");

            var json = config.Build();

            string[] connections = new string[]
            {
                json.GetConnectionString("AltConnection1"),
                json.GetConnectionString("AltConnection2"),
                json.GetConnectionString("AltConnection3"),
            };

            for (byte i = 0; i < connections.Length; i++)
            {
                var str = connections[i];
                if (CheckConnection(str))
                {
                    ActualConnectionString = str;
                    File.WriteAllText(FILE_NAME, str);
                    connections = null;
                    return true;
                }
            }
            return false;
        }
        private bool CheckConnection(string str)
        {
            SqlConnection conn = new SqlConnection(str);
            try
            {
                conn.Open();
                return true;
            }
            catch (SqlException ex) when (ex.Number == 4060)
            {
                return true;
            }
            catch (SqlException e1) when (e1.Number != -1)
            {
                return false;
            }

            finally
            {
                conn.Dispose();
            }
        }
    }
}
