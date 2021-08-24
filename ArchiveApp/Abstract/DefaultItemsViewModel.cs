using ArchiveApp.Abstract;
using ArchiveApp.Resources.Components;
using ArchiveApp.Services;
using ArchiveApp.ViewModels;
using ArchiveApp.Views;
using BL.Abstract;
using BL.DbHandling;
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
        protected readonly PageService pageService;
        protected readonly DropDownDataService dataService;
        protected readonly UnitOfWork handler;

        public DefaultItemsViewModel(PageService pageService,
                                     DropDownDataService dataService,
                                     UnitOfWork handler)
        {
            this.pageService = pageService;
            this.dataService = dataService;
            this.handler = handler;
            PropertyChanged += DefaultItemsViewModel_PropertyChanged;
            Init();
        }

        private void DefaultItemsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SelectedItem) && _nonNotifyProps.Contains(e.PropertyName))
            {
                if (selectedItem != null && selectedItem is T t)
                {
                    ItemSelected(t);
                    _nonNotifyProps.Remove(e.PropertyName);
                }
            }
        }

        public ViewBase View { get; private set; }


        private async void Init()
        {
            InitView();
            await Reload();
        }

        private void InitView()
        {
            var builder = new ColumnsBuilder<T>();
            OnSetupView(builder);

            var gridView = new GridView();

            allColumns = builder.GetColumnComponents();

            for (int i = 0; i < allColumns.Length; i++)
            {
                var col = allColumns[i];
                gridView.Columns.Add(col.Column);
            }


            actualColumns = gridView.Columns;
            removedColumns.Clear();
            SetupFilters(allColumns);
            View = gridView;
        }




        #region Данные

        private List<string> _nonNotifyProps = new List<string>();

        protected void SetupNonNotifyProperty(string name)
        {
            if (!_nonNotifyProps.Contains(name))
            {
                _nonNotifyProps.Add(name);
            }
        }

        public IList SelectedItems { get; set; }
        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }

        private object selectedItem;

        public ICollectionView ItemsView { get; protected set; }
        public ObservableCollection<T> Items { get; protected set; }


        #endregion

        #region Команды и методы

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
        protected async Task Reload()
        {
            LoadingAnimation = true;
            //Items = new ObservableCollection<T>(await OnLoadItems(appContext));
            Items = new ObservableCollection<T>(await LoadItems());
            RefreshCollectionView();
            LoadingAnimation = false;
        }

        protected abstract Task<IEnumerable<T>> LoadItems();

        protected abstract void OnAdd();
        protected abstract void OnEdit(T item);
        protected abstract void OnRemove(T[] items);

        public bool LoadingAnimation { get; private set; }




        private ICommand addCommand;
        private ICommand editCommand;
        private ICommand removeCommand;
        private ICommand updateCommand;

        public ICommand AddCommand => addCommand ??= new Command(x => Add());

        public ICommand EditCommand => editCommand ??= new Command(x => Edit(), y => SelectedItem != null);

        public ICommand RemoveCommand => removeCommand ??= new Command(x => Remove(), y => SelectedItem != null);

        public ICommand UpdateCommand => updateCommand ??= new CommandAsync(async x => await Reload());

        #endregion


        protected void RefreshCollectionView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            RefreshGrouping();
            RefreshFilter();
        }

        protected virtual void ItemSelected(T item) { }


        protected virtual void OnChangePage(PageService pageService)
        {
            pageService.RemovePage<Views.DefaultItemsView>();
            pageService.ChangePage<Views.DefaultItemsView>(DisappearAnimation.Default);
        }


        public void ChangePage()
        {
            OnChangePage(pageService);
        }


        #region Настрока столбцов

        private List<GridViewColumn> removedColumns = new List<GridViewColumn>();
        private GridViewColumnCollection actualColumns;
        private ColumnComponent[] allColumns;

        public ColumnComponent[] ColumnComponents { get; set; }

        private ICommand columnsCommand;
        public ICommand ColumnsCommand => columnsCommand ??= new Command(x =>
        {
            //todo Разобраться со столбцами
            if (!CheckWindow<ColumnsWindow>())
            {

                var vm = new FieldsViewModel();
                vm.SetupColumns(allColumns);

                vm.ChangeVisible = new Command(v =>
                {
                    if (v is ColumnComponent comp)
                    {
                        if (comp.IsVisible)
                        {
                            actualColumns.Add(comp.Column);
                        }
                        else
                        {
                            actualColumns.Remove(comp.Column);
                        }
                    }
                });

                vm.HideAllColumns = new Command(v =>
                {
                    for (int i = 0; i < allColumns.Length; i++)
                    {
                        allColumns[i].IsVisible = false;
                    }

                    actualColumns.Clear();
                    vm.SetupVisibleForAll(false);
                });

                vm.ShowAllColumns = new Command(v =>
                {
                    for (int i = 0; i < allColumns.Length; i++)
                    {
                        allColumns[i].IsVisible = true;
                        actualColumns.Add(allColumns[i].Column);
                    }

                    vm.SetupVisibleForAll(true);
                });

                var window = GetNewWindow<ColumnsWindow>(vm);
                window.Show();
            }
            else
            {
                Focus<ColumnsWindow>();
            }
        });

        protected abstract void OnSetupView(ColumnsBuilder<T> builder);

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
            if (!CheckWindow<GroupWindow>())
            {

                var context = new GroupSettingsViewModel();
                context.IsGrouping = GroupingColumn != null;
                context.SelectedColumn = GroupingColumn;
                context.Columns = allColumns;

                var win = GetNewWindow<GroupWindow>(context);


                context.Accept = new Command(y =>
                {
                    ItemsView.GroupDescriptions.Clear();
                    GroupingColumn = context.IsGrouping ? context.SelectedColumn : null;
                    RefreshGrouping();
                    win.Close();
                }, z => context.SelectedColumn != null);

                win.Show();
            }
            else
            {
                Focus<GroupWindow>();
            }
            
        });
        #endregion

        private List<Window> _wins = new List<Window>();

        private TWindow GetNewWindow<TWindow>(object dataContext)
            where TWindow: Window, new()
        {

            var dc = dataContext;
            var win = new TWindow();
            win.DataContext = dc;
            win.Closing += (a, b) => _wins.Remove(win);
            _wins.Add(win);
            return win;
        }

        private bool CheckWindow<TWindow>()
            where TWindow: Window, new()
        {
            return _wins?.Any(x => x.GetType() == typeof(TWindow)) ?? false;
        }

        private void Focus<TWindow>()
            where TWindow: Window, new()
        {
            var win = _wins.FirstOrDefault(y => y.GetType() == typeof(TWindow));
            win?.Focus();
        }


        #region Фильтр

        private ICommand showFiltersWindowCommand;
        private void SetupFilters(ColumnComponent[] allColumns)
        {
            var existFilters = new List<FilterOption>();
            filters = allColumns.Select(y => 
            { 
                y.FilterOption.FilterValueChanged += Filter_FilterValueChanged;
                return y.FilterOption;
            }).ToArray();
        }
        public int FiltersCount { get; private set; }

        List<IFilterOption> actualFilters = new List<IFilterOption>();

        private void Filter_FilterValueChanged(IFilterOption obj)
        {
            if (obj.IsClear)
            {
                actualFilters.Remove(obj);
            }
            else if(!actualFilters.Contains(obj))
            {
                actualFilters.Add(obj);
            }
            RefreshFilter();
        }

        public void RefreshFilter()
        {
            ItemsView.Filter = item =>
            {
                return actualFilters.All(f => f.Filter(item));
            };

        }

        private void Vm_FilterCountChanged(int obj)
        {
            FiltersCount = obj;

            if(FiltersCount == 0)
            {
                ItemsView.Filter = null;
                actualFilters.Clear();
            }
        }

        public ICommand ShowFiltersWindowCommand => showFiltersWindowCommand ??= new Command(x =>
        {
            if (!CheckWindow<FiltersWindow>())
            {
                var vm = new FiltersViewModel();
                vm.SetupFilterOptions(filters);
                vm.FilterCountChanged += Vm_FilterCountChanged;
                var win = GetNewWindow<FiltersWindow>(vm);
                win.Show();
            }
            else
            {
                Focus<FiltersWindow>();
            }
        });


        FilterOption[] filters;

        #endregion
    }
}
