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
            if (item.OrganId.HasValue)
            {
                item.Organ = null;
            }
            if (item.SocialId.HasValue)
            {
                item.Social = null;
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

        public async Task<IEnumerable<Protocol>> LoadItemsAsync()
        {
            await context.Protocols.
                Include(p => p.People).ThenInclude(x => x.Natio).
                Include(x => x.People).ThenInclude(x => x.Education).
                Include(x => x.People).ThenInclude(x => x.FamilyType).
                Include(x => x.People).ThenInclude(x => x.Party).
                Include(x => x.Organ).Include(x => x.Social)
                .LoadAsync();

            return context.Protocols.
                Include(p => p.People).ThenInclude(x => x.Natio).
                Include(x => x.People).ThenInclude(x => x.Education).
                Include(x => x.People).ThenInclude(x => x.FamilyType).
                Include(x => x.People).ThenInclude(x => x.Party).
                Include(x => x.Organ).Include(x => x.Social);
        }

        public bool Update(Protocol item)
        {
            try
            {
                Clear(item);
                context.Protocols.Update(item);
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
            context.Protocols.Include(x => x.People).ThenInclude(x => x.Natio).Load();
            return context.Protocols.Include(x => x.People).ThenInclude(x => x.Natio);
        }
    }
}
