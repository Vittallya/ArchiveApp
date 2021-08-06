using System;
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
        public object Item { get; set; }
        public ICollectionView Items { get; }
        public bool LoadingAnimation { get; }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand UpdateCommand { get; }

        public ViewBase View { get; }

        public void ChangePage();
    }
}
