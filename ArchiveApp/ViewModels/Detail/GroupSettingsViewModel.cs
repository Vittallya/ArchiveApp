using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;
using ArchiveApp.Resources.Components;
using System.Linq;
using System.Windows.Input;

namespace ArchiveApp.ViewModels
{
    public class GroupSettingsViewModel: BaseViewModel
    {
        public ICommand Accept { get; set; }
        public ICommand ClearGrouping { get; set; }
        public ICommand SetupGrouping => new Command(x =>
        {
            SelectedColumn = Columns.FirstOrDefault();
        });

        public bool IsGrouping { get; set; }

        public ColumnComponent[] Columns { get; set; }

        public ColumnComponent SelectedColumn { get; set; }

    }
}
