using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Models.Comparers
{
    public class GenericComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object> propGetter;

        public GenericComparer(Func<T, object> propGetter)
        {
            this.propGetter = propGetter;
        }

        public bool Equals([AllowNull] T x, [AllowNull] T y)
        {
            return propGetter(x)?.Equals(propGetter(y)) ?? false;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return propGetter(obj)?.GetHashCode() ?? 0;
        }
    }
}
