using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BL.DbHandling
{
    public class AppContextLoader
    {
        public bool IsCompleted { get; private set; }

        private Thread thread;
        private readonly DbConnectionHandler db;

        public AppContextLoader(DbConnectionHandler db)
        {
            this.db = db;
        }

        public string Message { get; private set; }
        public bool Result { get; private set; }

        public async Task TryLoad()
        {
            thread = new Thread(LoadMethod);
            thread.Start();            

            while (!IsCompleted)
            {
                await Task.Delay(100);
            }
        }

        private void LoadMethod()
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(db.ActualConnectionString);
            Models.AppContext appContext = default;
            try
            {
                appContext = new Models.AppContext(builder.Options);
                appContext.Peoples.Load();
                Result = true;
            }
            catch(Exception ex)
            {
                Message = ex.Message;
                Result = false;
            }
            finally
            {
                IsCompleted = true;
                appContext?.Dispose();
            }
        }
    }
}
