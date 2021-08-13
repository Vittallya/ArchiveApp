using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL.Abstract
{
    public interface IDataHandler<T>
    {
        public bool Add(T item);

        public Task<bool> Update(T item);

        public bool Remove(T[] item, bool isRemoveAll);
        public void ClearTracking();

        public string Message { get; }

        public bool Result { get; }

        public Task<bool> SaveChanges();
    }
}
