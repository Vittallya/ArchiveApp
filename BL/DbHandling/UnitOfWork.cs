using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task ReloadProtocol(Protocol protocol)
        {
            await Protocols.LoadDataAsync(protocol);
            await Peoples.LoadDataAsync(protocol.People);
        }

        public IDataHandler<People> Peoples => peopleRepo ??= new PeopleDataHandler(db);
        public IDataHandler<Protocol> Protocols => protocolRepo ??= new ProtocolDataHandler(db);

        public IEnumerable<People> PeoplesClear => db.Peoples;

        public IEnumerable<Protocol> ProtocolsClear => db.Protocols;

        public IEnumerable<Natio> Natios => db.Set<Natio>();
        public IEnumerable<Party> Parties =>  db.Set<Party>();
        public IEnumerable<Social> Socials =>  db.Set<Social>();
        public IEnumerable<Organ> Organs =>  db.Set<Organ>();
        public IEnumerable<FamilyType> FamilyTypes =>  db.Set<FamilyType>();
        public IEnumerable<Education> Educations => db.Set<Education>();
        public string Message { get; private set; }



        public SaveChangesResult Save()
        {
            try
            {
                db.SaveChanges();
                db.ChangeTracker.Clear();
                return new SaveChangesResult(true, null, SaveChangesResultType.Ok);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new SaveChangesResult(false, ex, SaveChangesResultType.NeedUpdate);
            }
            catch (Exception e)
            {
                Message = e.Message;
                return new SaveChangesResult(false, e, SaveChangesResultType.Error);
            }
        }

        public async Task<SaveChangesResult> SaveAsync()
        {
            try
            {
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                return new SaveChangesResult(true, null, SaveChangesResultType.Ok);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new SaveChangesResult(false, ex, SaveChangesResultType.NeedUpdate);
            }
            catch (Exception e)
            {
                Message = e.Message;
                return new SaveChangesResult(false, e, SaveChangesResultType.Error);
            }
        }

        public bool RemoveProtocols(Protocol[] items, bool isRemoveAll)
        {
            try
            {
                if (isRemoveAll)
                {
                    Peoples.Remove(items.Select(x => x.People).ToArray());
                }
                Protocols.Remove(items);
                return true;
            }
            catch(Exception e)
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
