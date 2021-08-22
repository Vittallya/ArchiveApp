using BL.Abstract;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool Remove(People[] items, bool isRemoveAll)
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
            return db.Peoples.Include(x => x.Natio);
        }
    }
}
