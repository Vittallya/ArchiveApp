using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArchiveApp.Abstract
{
    public interface IDefaultItemsViewModel
    {
        public object SelectedItem { get; set; }
        public ICollectionView ItemsView { get; }
        public bool LoadingAnimation { get; }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand UpdateCommand { get; }

        public ViewBase View { get; }
        public IList SelectedItems { get; set; }

        public void ChangePage();
    }
}
