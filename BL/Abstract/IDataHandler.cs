﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL.Abstract
{
    public interface IDataHandler<T>
    {
        public bool Add(T item);

        public bool Update(T item);

        public bool Remove(T[] item, bool isRemoveAll);

        public string Message { get; }

        public Task<IEnumerable<T>> LoadItemsAsync();
        public IEnumerable<T> LoadItems();
        public bool Result { get; }
    }
}
