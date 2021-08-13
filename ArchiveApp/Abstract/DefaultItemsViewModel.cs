using ArchiveApp.Abstract;
using BL.Abstract;
using Microsoft.EntityFrameworkCore;
using Models;
using MVVM_Core;
using MVVM_Core.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AppContext = Models.AppContext;

namespace ArchiveApp.Abstract
{
    public abstract class DefaultItemsViewModel<T> : BaseViewModel, IDefaultItemsViewModel
        where T : class
    {
        protected readonly AppContext appContext;
        private readonly PageService pageService;
        protected readonly IDataHandler<T> handler;

        public DefaultItemsViewModel(AppContext appContext, PageService pageService, IDataHandler<T> handler)
        {
            this.appContext = appContext;
            this.pageService = pageService;
            this.handler = handler;
            Init();
        }

        public ViewBase View { get; private set; }

        protected virtual Dictionary<string, string> Colums { get; set; }

        protected virtual ViewBase OnSetupView()
        {
            if(Colums == null)
            {
                throw new ArgumentException("Either override this method either override filed colums");
            }

            var gridView = new GridView();
            

            foreach(var col in Colums)
            {
                var column = new GridViewColumn() { Header = col.Key, DisplayMemberBinding = new Binding(col.Value) };                
                gridView.Columns.Add(column);
            }
            return gridView;
        }

        private void Init()
        {
            View = OnSetupView();
            Reload();
        }

        public object SelectedItem 
        { 
            get => selectedItem;
            set
            {                 
                selectedItem = value;
                OnPropertyChanged();
                if (selectedItem != null && selectedItem is T t)
                    ItemSelected(t);
            }
        }

        public ICollectionView ItemsView { get; protected set; }
        public ObservableCollection<T> Items { get; protected set; }

        private void Add()
        {
            OnAdd();
        }

        private void Edit()
        {
            OnEdit(SelectedItem as T);
        }

        private void Remove()
        {
            if(MessageBox.Show("Подтвердить удаление?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                OnRemove(SelectedItems.OfType<T>().ToArray());
            }
        }


        public bool LoadingAnimation { get; private set; }

        private ICommand addCommand;
        private ICommand editCommand;
        private ICommand removeCommand;
        private ICommand updateCommand;
        private object selectedItem;

        public ICommand AddCommand => addCommand ??= new Command(x => Add());

        public ICommand EditCommand => editCommand ??= new Command(x => Edit(), y => SelectedItem != null);

        public ICommand RemoveCommand => removeCommand ??= new Command(x => Remove(), y => SelectedItem != null);

        public ICommand UpdateCommand => updateCommand ??= new Command(x => Reload());

        public IList SelectedItems { get; set; }

        protected async void Reload()
        {
            LoadingAnimation = true;
            await OnLoadItems(appContext);

            Items = new ObservableCollection<T>(appContext.Set<T>());

            if (ItemsView == null)
            {
                ItemsView = CollectionViewSource.GetDefaultView(Items);
            }

            LoadingAnimation = false;
        }

        protected virtual void ItemSelected(T item) { }

        protected virtual async Task OnLoadItems(AppContext appContext)
        {
            await appContext.Set<T>().LoadAsync();
        }

        protected virtual void OnChangePage(PageService pageService)
        {
            pageService.RemovePage<Views.DefaultItemsView>();
            pageService.ChangePage<Views.DefaultItemsView>(DisappearAnimation.Default);
        }


        public void ChangePage()
        {
            OnChangePage(pageService);
        }
        protected abstract void OnAdd();
        protected abstract void OnEdit(T item);
        protected abstract void OnRemove(T[] items);


        
    }
}
