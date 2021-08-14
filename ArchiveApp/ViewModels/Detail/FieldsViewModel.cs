using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using ArchiveApp.Resources.Components;
using System.Collections.ObjectModel;

namespace ArchiveApp.ViewModels
{
    public class FieldsViewModel : BaseViewModel
    {
        public ObservableCollection<ColumnComponent> Columns { get; private set; }
        public ICommand ChangeVisible { get; set; }
        public ICommand HideAllColumns { get; set; }
        public ICommand ShowAllColumns { get; set; }

        public void SetupVisibleForAll(bool vis)
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                var col = Columns[i];
                Columns[i] = new ColumnComponent { Column = col.Column, Header = col.Header, IsVisible = vis };
            }
        }

        public void SetupColumns(IEnumerable<ColumnComponent> columns)
        {
            Columns = new ObservableCollection<ColumnComponent>(columns);
        }
    }
}
