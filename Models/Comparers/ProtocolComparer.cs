using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Models.Comparers
{
    public class ProtocolComparer : IEqualityComparer<Protocol>
    {
        private readonly Func<Protocol, object> propertyFunc;

        public ProtocolComparer(Func<Protocol, object> propertyFunc)
        {
            this.propertyFunc = propertyFunc;
        }

        public ProtocolComparer()
        {
            propertyFunc = p => p.Id;
        }

        public bool Equals([AllowNull] Protocol x, [AllowNull] Protocol y)
        {
            return propertyFunc(x).Equals(propertyFunc(y));
        }

        public int GetHashCode([DisallowNull] Protocol obj)
        {
            return propertyFunc(obj).GetHashCode();
        }
    }
}
