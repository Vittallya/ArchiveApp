using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BL.Abstract
{
    public interface IDataHandler<T>
    {
        public bool Add(T item);

        public bool Update(T item);

        public bool Remove(T[] item);

        public Task LoadDataAsync(T item);
        public void LoadData(T item);

        public string Message { get; }

        public T Find(object id);
        public Task<T> FindAsync(object id);
        public Task<IEnumerable<T>> LoadItemsAsync();
        public IEnumerable<T> LoadItems();
        public IEnumerable<T> LoadItems(Expression<Func<T, object>> include);
        public bool Result { get; }
    }
}
