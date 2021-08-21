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
    class ProtocolDataHandler : DataHandler, IDataHandler<Protocol>
    {

        public bool Result => throw new NotImplementedException();

        public ProtocolDataHandler(AppContext context): base(context)
        {
        }

        private void Clear(Protocol item)
        {

            //if (item.Organ.Id > 0)
            //{
            //    item.OrganId = item.Organ.Id;
            //    item.Organ = null;
            //}
            //if(item.Social.Id > 0)
            //{
            //    item.SocialId = item.Social.Id;
            //    item.Social = null;
            //}


            //if (item.People.Nationality.Id > 0)
            //{
            //    item.People.NationalityId = item.People.Nationality.Id;
            //    item.People.Nationality = null;
            //}

            //if(item.People.Party.Id > 0)
            //{
            //    item.People.PartyId = item.People.Party.Id;
            //    item.People.Party = null;
            //}

            //if(item.People.Family.Id > 0)
            //{
            //    item.People.FamilyId = item.People.Family.Id;
            //    item.People.Family = null;
            //}

            //if(item.People.Education.Id > 0)
            //{
            //    item.People.EducationId = item.People.Education.Id;
            //    item.People.Education = null;
            //}
            //var people = item.People;

            //if (item.People.Id > 0)
            //{
            //    item.PeopleId = item.People.Id;
            //    item.People = null;
            //}
        }

        public bool Add(Protocol item)
        {
            try
            {
                Clear(item);
                context.Protocols.Add(item);
                return true;
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public async Task<IEnumerable<Protocol>> LoadItemsAsync()
        {
            await context.Protocols.Include(p => p.People).LoadAsync();
            return context.Protocols.Include(p => p.People);
        }

        public bool Update(Protocol item)
        {
            try
            {
                //invoker?.Invoke();

                context.Protocols.Update(item);
                //await using(var context = new AppContextFactory().CreateDbContext(null))
                //{
                //    context.Protocols.Update(item);
                //    await context.SaveChangesAsync();
                //}

                //invoker = () =>
                //{
                //    context.Entry(item.People).State = EntityState.Detached;
                //    context.Entry(item).State = EntityState.Detached;
                //};
                return true;
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
        }


        public bool Remove(Protocol[] items, bool isRemoveAll)
        {
            //using (var context = new AppContextFactory().CreateDbContext(null))
            //{
                if (isRemoveAll)
                {
                    context.RemoveRange(items.Select(x => x.People).ToArray());
                }

                items = items.Select(x =>
                {
                    x.People = null;
                    return x;
                }).ToArray();

                context.Protocols.RemoveRange(items);
                context.SaveChanges();
            //}
            return true;
        }

        public IEnumerable<Protocol> LoadItems()
        {
            context.Protocols.Include(x => x.People).Load();
            return context.Protocols.Include(x => x.People);
        }
    }
}
