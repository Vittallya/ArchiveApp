using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class DbConnectionHandler
    {
        public const string FILE_NAME = "connection.txt";

        private readonly IConfigurationRoot root;

        public string ActualConnectionString { get; private set; }

        private string[] connections;

        public DbConnectionHandler(IConfigurationRoot root)
        {
            this.root = root;
        }

        public string Message { get; private set; }
        public string DetailMessage { get; private set; }

        public async Task<bool> TryConnect()
        {
            if (File.Exists(FILE_NAME) )
            {
                string conn = File.ReadAllText(FILE_NAME);
                if(await CheckConnection(conn))
                {
                    ActualConnectionString = conn;
                    return true;
                }
            }

            connections = new string[]
            {                
                root.GetConnectionString("AltConnection1"),
                root.GetConnectionString("AltConnection2"),
                root.GetConnectionString("AltConnection3"),
            };

            for (byte i = 0; i <= connections.Length; i++)
            {
                var str = connections[i];
                if (await CheckConnection(str))
                {
                    ActualConnectionString = str;
                    File.WriteAllText(FILE_NAME, str);
                    connections = null;
                    return true;
                }
            }


            Message = "Не найден экземляр сервера либо он не включен";
            connections = null;
            return false;
        }        

        private async Task<bool> CheckConnection(string str) 
        {
            SqlConnection conn = new SqlConnection(str);
            try
            {
                await conn.OpenAsync();
                return true;
            }
            catch (SqlException e1) when (e1.Number != -1 && e1.Number != 4060)
            {
                Message = e1.Message;
                DetailMessage = e1.InnerException?.Message;
                return false;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }
    }
}
