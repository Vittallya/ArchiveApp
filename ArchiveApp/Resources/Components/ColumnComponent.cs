using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace ArchiveApp.Resources.Components
{
    public class ColumnComponent
    {
        public int Number { get; set; }

        public int Order { get; set; }
        public bool IsVisible { get; set; }

        public string Header { get; set; }

        public string Binding { get; set; }
        public IValueConverter Converter { get; set; }
    }
}
