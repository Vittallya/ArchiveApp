using ArchiveApp.Abstract;
using ArchiveApp.Resources.Components;
using ArchiveApp.ViewModels;
using ArchiveApp.Views;
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

        protected virtual Dictionary<string, string> Columns { get; set; }


        private void Init()
        {
            InitView();
            Reload();
        }

        private void InitView()
        {
            View = OnSetupView();
            if (View is GridView gridView)
            {
                actualColumns = gridView.Columns;
                removedColumns.Clear();
                allColumns = actualColumns.Select(x => new ColumnComponent
                { Column = x, Header = x.Header, BindingProperty = (x.DisplayMemberBinding as Binding)?.Path?.Path }).
                ToArray();
                SetupFilters(allColumns);
            }
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
            if (MessageBox.Show("Подтвердить удаление?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
            Items = new ObservableCollection<T>(await OnLoadItems(appContext));
            RefreshCollectionView();
            LoadingAnimation = false;
        }

        protected void RefreshCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            RefreshGrouping();
        }

        protected virtual void ItemSelected(T item) { }

        protected virtual async Task<IEnumerable<T>> OnLoadItems(AppContext appContext)
        {
            return appContext.Set<T>();
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


        #region Настрока столбцов

        private List<GridViewColumn> removedColumns = new List<GridViewColumn>();
        private GridViewColumnCollection actualColumns;
        private ColumnComponent[] allColumns;

        public ColumnComponent[] ColumnComponents { get; set; }

        private ICommand columnsCommand;
        public ICommand ColumnsCommand => columnsCommand ??= new Command(x =>
        {
            var vm = new FieldsViewModel();
            vm.SetupColumns(actualColumns.
                Select(x => new ColumnComponent { Header = x.Header, IsVisible = true, Column = x }).
                Union(removedColumns.Select(y => new ColumnComponent { Header = y.Header, IsVisible = false, Column = y })));

            vm.ChangeVisible = new Command(v =>
            {
                if (v is ColumnComponent comp)
                {
                    if (comp.IsVisible)
                    {
                        actualColumns.Add(comp.Column);
                        removedColumns.Remove(comp.Column);
                    }
                    else
                    {
                        actualColumns.Remove(comp.Column);
                        removedColumns.Add(comp.Column);
                    }
                }
            });

            vm.HideAllColumns = new Command(v =>
            {
                removedColumns.AddRange(actualColumns);
                actualColumns.Clear();
                vm.SetupVisibleForAll(false);
            });

            vm.ShowAllColumns = new Command(v =>
            {
                foreach (var col in removedColumns)
                {
                    actualColumns.Add(col);
                }

                removedColumns.Clear();
                vm.SetupVisibleForAll(true);
            });

            ColumnsWindow window = new ColumnsWindow();
            window.DataContext = vm;
            window.Show();
        });

        protected virtual ViewBase OnSetupView()
        {
            if (Columns == null)
            {
                throw new ArgumentException("Either override this method either override filed colums");
            }

            var gridView = new GridView();


            foreach (var col in Columns)
            {
                var column = new GridViewColumn() { Header = col.Key, DisplayMemberBinding = new Binding(col.Value) };
                gridView.Columns.Add(column);
            }

            return gridView;
        }

        #endregion

        #region группировка

        private ICommand setupGroupingCommand;
        public ColumnComponent GroupingColumn { get; private set; }

        public void RefreshGrouping()
        {
            ItemsView.GroupDescriptions.Clear();
            if (GroupingColumn != null)
            {
                ItemsView.GroupDescriptions.Add(new PropertyGroupDescription(GroupingColumn.BindingProperty,
                    (GroupingColumn.Column.DisplayMemberBinding as Binding)?.Converter));
            }
            ItemsView.Refresh();
        }

        public ICommand SetupGroupingCommand => setupGroupingCommand ??= new Command(x =>
        {
            var context = new GroupSettingsViewModel();
            context.IsGrouping = GroupingColumn != null;
            context.SelectedColumn = GroupingColumn;
            context.Columns = allColumns;

            var win = new GroupWindow();

            context.Accept = new Command(y =>
            {
                ItemsView.GroupDescriptions.Clear();
                GroupingColumn = context.IsGrouping ? context.SelectedColumn : null;
                RefreshGrouping();
                win.Close();
            }, z => context.SelectedColumn != null);

            win.DataContext = context;
            win.Show();
        });
        #endregion

        #region Фильтр

        private ICommand showFiltersWindowCommand;
        private void SetupFilters(ColumnComponent[] allColumns)
        {
            filters = allColumns.Select(y =>
            {
                var filter = FilerOptionSource.GetFilter<T>(y.BindingProperty, y.Header?.ToString());
                filter.FilterValueChanged += Filter_FilterValueChanged;
                return filter;
            }).
            ToArray();
        }

        List<IFilterOption> actualFilters = new List<IFilterOption>();

        private void Filter_FilterValueChanged(IFilterOption obj)
        {
            if (obj.FilterControls.All(c => c.IsClear))
            {
                actualFilters.Remove(obj);
            }
            else if(!actualFilters.Contains(obj))
            {
                actualFilters.Add(obj);
            }

            ItemsView.Filter = item => actualFilters.All(f => f.Filter(item));
            RefreshCollectionView();
        }

        public ICommand ShowFiltersWindowCommand => showFiltersWindowCommand ??= new Command(x =>
        {
            var vm = new FiltersViewModel();
            vm.FilterOptions = filters;

            var win = new FiltersWindow();
            win.DataContext = vm;
            win.Show();
        });

        FilterOption[] filters;

        #endregion
    }
}
