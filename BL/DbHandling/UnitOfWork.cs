using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BL.Abstract;
using Microsoft.EntityFrameworkCore;
using Models;
using AppContext = Models.AppContext;

namespace BL.DbHandling
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppContext db;
        private IEnumerable<Natio> natios;

        public UnitOfWork(AppContext db)
        {
            this.db = db;
        }


        private IDataHandler<People> peopleRepo;
        private IDataHandler<Protocol> protocolRepo;

        public bool UpdateProtocol(Protocol protocol)
        {
            try
            {
                var copy = protocol.Clone() as Protocol;
                if (copy.People != null)
                {
                    Peoples.Update(copy.People);
                    if(copy.PeopleId > 0)
                        copy.People = null;
                }

                Protocols.Update(copy);
                return true;
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public IDataHandler<People> Peoples => peopleRepo ??= new PeopleDataHandler(db);
        public IDataHandler<Protocol> Protocols => protocolRepo ??= new ProtocolDataHandler(db);



        public IEnumerable<Natio> Natios => db.Set<Natio>();
        public IEnumerable<Party> Parties =>  db.Set<Party>();
        public IEnumerable<Social> Socials =>  db.Set<Social>();
        public IEnumerable<Organ> Organs =>  db.Set<Organ>();
        public IEnumerable<FamilyType> FamilyTypes =>  db.Set<FamilyType>();
        public IEnumerable<Education> Educations => db.Set<Education>();
        public string Message { get; private set; }

        public bool Save()
        {
            try
            {
                db.SaveChanges();
                db.ChangeTracker.Clear();
                return true;
            }
            catch (Exception e)
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
