using BL.Abstract;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
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

        public bool Add(People item)
        {
            try
            {
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
    }
}
