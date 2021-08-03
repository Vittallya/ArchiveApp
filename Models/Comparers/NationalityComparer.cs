using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Models.Comparers
{
    class NationalityComparer : IEqualityComparer<Nationality>
    {
        public bool Equals([AllowNull] Nationality x, [AllowNull] Nationality y)
        {
            return (x?.Id == y?.Id);
        }

        public int GetHashCode([DisallowNull] Nationality obj)
        {
            return obj?.Id ?? 0;
        }
    }
}
