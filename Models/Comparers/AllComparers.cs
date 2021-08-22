using Models;
using Models.Comparers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public static class AllComparers
    {
        public static IEqualityComparer<Organ> GetOrganComparer => new OrganComparer();
        public static IEqualityComparer<Natio> GetNationalityComparer => new NationalityComparer();
    }
}
