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
            DisplayLower = display.ToLower();
        }

        internal string Display { get; }
        internal string DisplayLower { get; }

        internal object Item { get; }
    }
}
