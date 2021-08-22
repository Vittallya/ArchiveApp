using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Models.Comparers
{
    class NationalityComparer : IEqualityComparer<Natio>
    {
        public bool Equals([AllowNull] Natio x, [AllowNull] Natio y)
        {
            return (x?.Id == y?.Id);
        }

        public int GetHashCode([DisallowNull] Natio obj)
        {
            return obj?.Id ?? 0;
        }
    }
}
