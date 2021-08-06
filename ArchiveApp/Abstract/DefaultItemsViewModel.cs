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
        private readonly AppContext appContext;
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

        public object Item { get; set; }

        public ICollectionView Items { get; protected set; }

        public virtual async Task OnAdd()
        {

        }
        public virtual async Task OnEdit()
        {

        }
        
        public virtual async Task OnRemove()
        {

        }

        public virtual async Task OnUpdate()
        {

        }

        public bool LoadingAnimation { get; private set; }

        private ICommand addCommand;
        private ICommand editCommand;
        private ICommand removeCommand;
        private ICommand updateCommand;

        public ICommand AddCommand => addCommand ?? (addCommand = new CommandAsync(async x => await OnAdd()));

        public ICommand EditCommand => editCommand ?? (editCommand = new CommandAsync(async x => await OnEdit(), y => Item != null));

        public ICommand RemoveCommand => removeCommand ?? (removeCommand = new CommandAsync(async x => await OnRemove(), y => Item != null));

        public ICommand UpdateCommand => updateCommand ?? (updateCommand = new CommandAsync(async x => await OnAdd()));

        protected async Task Reload()
        {
            LoadingAnimation = true;
            await OnLoadItems(appContext);
            Items = CollectionViewSource.GetDefaultView(
                new ObservableCollection<T>(appContext.Set<T>().Local));

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
    }
}
