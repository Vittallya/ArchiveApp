using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveApp.Resources
{
    struct DisplayItem
    {
        internal DisplayItem(string display, object item)
        {
            Display = display;
            Item = item;
        }

        internal string Display { get; }

        internal object Item { get; }
    }
}
