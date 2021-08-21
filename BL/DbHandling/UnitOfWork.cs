using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BL.Abstract;
using Models;
using AppContext = Models.AppContext;

namespace BL.DbHandling
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppContext db;

        public UnitOfWork(AppContext db)
        {
            this.db = db;
        }


        private IDataHandler<People> peopleRepo;
        private IDataHandler<Protocol> protocolRepo;



        public IDataHandler<People> Peoples => peopleRepo ??= new PeopleDataHandler(db);
        public IDataHandler<Protocol> Protocols => protocolRepo ??= new ProtocolDataHandler(db);

        public string Message { get; private set; }

        public bool Save()
        {
            try
            {
                db.SaveChanges();
                db.ChangeTracker.Clear();
                return true;
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }
        }


        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
