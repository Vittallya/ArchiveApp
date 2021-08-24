using BL.Abstract;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AppContext = Models.AppContext;

namespace BL.DbHandling
{
    class PeopleDataHandler : IDataHandler<People>
    {
        private readonly AppContext db;

        public string Message { get; private set; }

        public bool Result { get; private set; }

        public PeopleDataHandler(AppContext db)
        {
            this.db = db;
        }

        private void Clear(People item)
        {
            if (item.NatioId.HasValue)
            {
                item.Natio = null;
            }

            if (item.EducationId.HasValue)
            {
                item.Education = null;
            }

            if (item.PartyId.HasValue)
            {
                item.Party = null;
            }

            if (item.FamilyTypeId.HasValue)
            {
                item.FamilyType = null;
            }
        }

        public bool Add(People item)
        {
            try
            {
                Clear(item);
                db.Peoples.Add(item);
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public async Task<IEnumerable<People>> LoadItemsAsync()
        {
            await db.Peoples.LoadAsync();
            return db.Peoples;
        }

        public bool Remove(People[] items)
        {
            try
            {
                items.All(y => { Clear(y); return true; });
                db.Peoples.RemoveRange(items);
                return true;
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public bool Update(People item)
        {
            try
            {
                Clear(item);
                db.Peoples.Update(item);
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public IEnumerable<People> LoadItems()
        {
            return db.Peoples;
        }

        public async Task LoadDataAsync(People item)
        {
            db.ChangeTracker.Clear();
            await db.Entry(item).ReloadAsync();
            db.Entry(item).State = EntityState.Detached;
        }

        public void LoadData(People item)
        {
            db.ChangeTracker.Clear();
            db.Entry(item).Reload();
            db.Entry(item).State = EntityState.Detached;
        }

        public People Find(object id)
        {
            return db.Peoples.Find(id);
        }

        public async Task<People> FindAsync(object id)
        {
            return await db.Peoples.FindAsync(id);
        }

        public IEnumerable<People> LoadItems(Expression<Func<People, object>> include)
        {
            return db.Peoples.Include(include);
        }
    }
}
