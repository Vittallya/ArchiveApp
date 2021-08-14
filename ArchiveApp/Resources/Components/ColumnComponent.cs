using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArchiveApp.Resources.Components
{
    public class ColumnComponent
    {
        public bool IsVisible { get; set; }

        public object Header { get; set; }
        public string BindingProperty { get; set; }
        public GridViewColumn Column { get; set; }
    }
}
