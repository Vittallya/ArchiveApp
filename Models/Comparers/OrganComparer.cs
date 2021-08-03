using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Models.Comparers
{
    class OrganComparer : IEqualityComparer<Organ>
    {
        public bool Equals([AllowNull] Organ x, [AllowNull] Organ y)
        {
            return (x?.Id == y?.Id);
        }

        public int GetHashCode([DisallowNull] Organ obj)
        {
            return obj?.Id ?? 0;
        }
    }
}
