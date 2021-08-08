﻿using ArchiveApp.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArchiveApp.Views
{
    /// <summary>
    /// Логика взаимодействия для DefaultItemsView.xaml
    /// </summary>
    public partial class DefaultItemsView : Page
    {
        public DefaultItemsView()
        {
            InitializeComponent();
            if(this.DataContext is IDefaultItemsViewModel vm)
            {
                vm.SelectedItems = listView.SelectedItems;
            }
        }
    }
}
