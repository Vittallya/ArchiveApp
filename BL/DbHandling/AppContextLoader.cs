﻿using Microsoft.EntityFrameworkCore;
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

        public AppContextLoader()
        {
        }

        public string Message { get; private set; }
        public bool Result { get; private set; }

        private bool _createDb;

        public async Task TryLoad(bool createDb)
        {
            IsCompleted = false;
            _createDb = createDb;
            thread = new Thread(LoadMethod);
            thread.Start();            

            while (!IsCompleted)
            {
                await Task.Delay(100);
            }
        }

        private void LoadMethod()
        {
            var factory = new AppContextFactory();

            Models.AppContext appContext = default;
            
            try
            {
                appContext = factory.CreateDbContext(null);

                if (_createDb)
                    appContext.Database.EnsureCreated();

                appContext.Protocols.Include(y => y.People).Load();
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
