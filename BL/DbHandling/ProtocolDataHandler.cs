using BL.Abstract;
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
            var people = item.People;

            if (item.People.Id > 0)
            {
                item.PeopleId = item.People.Id;
                item.People = null;
            }
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

        public async Task<bool> Update(Protocol item)
        {
            try
            {
                await using(var context = new AppContextFactory().CreateDbContext(null))
                {
                    context.Protocols.Update(item);
                    await context.SaveChangesAsync();
                }
                return true;
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        

        Action action;

        public bool Remove(Protocol[] items, bool isRemoveAll)
        {
            using (var context = new AppContextFactory().CreateDbContext(null))
            {
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
            }
            return true;
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public void ClearTracking()
        {
            action?.Invoke();
        }
    }
}
