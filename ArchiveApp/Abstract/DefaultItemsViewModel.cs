using ArchiveApp.Abstract;
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AppContext = Models.AppContext;

namespace ArchiveApp.Abstract
{
    public abstract class DefaultItemsViewModel<T> :BaseViewModel, IDefaultItemsViewModel
        where T: class
    {
        protected readonly AppContext appContext;
        private readonly PageService pageService;

        public DefaultItemsViewModel(AppContext appContext, PageService pageService)
        {
            this.appContext = appContext;
            this.pageService = pageService;
            Init();
        }

        public ViewBase View { get; private set; }

        protected abstract ViewBase OnSetupView();

        private async void Init()
        {
            View = OnSetupView();
            await Reload();
        }

        public object SelectedItem { get; set; }

        public ICollectionView ItemsView { get; protected set; }
        public ObservableCollection<T> Items { get; protected set; }

        private async Task Add()
        {
            await OnAdd();
        }

        private async Task Edit()
        {
            await OnEdit(SelectedItem as T);
        }
        
        private async Task Remove()
        {
            await OnRemove(SelectedItems.OfType<T>().ToArray());
        }


        public bool LoadingAnimation { get; private set; }

        private ICommand addCommand;
        private ICommand editCommand;
        private ICommand removeCommand;
        private ICommand updateCommand;

        public ICommand AddCommand => addCommand ?? (addCommand = new CommandAsync(async x => await Add()));

        public ICommand EditCommand => editCommand ?? (editCommand = new CommandAsync(async x => await Edit(), y => SelectedItem != null));

        public ICommand RemoveCommand => removeCommand ?? (removeCommand = new CommandAsync(async x => await Remove(), y => SelectedItem != null));

        public ICommand UpdateCommand => updateCommand ?? (updateCommand = new CommandAsync(async x => await Reload()));

        public IList SelectedItems { get; set; }

        protected async Task Reload()
        {
            LoadingAnimation = true;
            await OnLoadItems(appContext);
            Items = new ObservableCollection<T>(appContext.Set<T>().Local);
            ItemsView = CollectionViewSource.GetDefaultView(Items);

            LoadingAnimation = false;
        }

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
        protected abstract Task OnAdd();
        protected abstract Task OnEdit(T item);
        protected abstract Task OnRemove(T[] items);
    }
}
