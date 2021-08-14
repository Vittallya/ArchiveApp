using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ArchiveApp.Abstract;
using ArchiveApp.Locators;
using ArchiveApp.Resources.Components;
using ArchiveApp.Views;
using BL.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using MVVM_Core;
using MVVM_Core.Validation;
using AppContext = Models.AppContext;

namespace ArchiveApp.ViewModels
{
    public class ProtocolItemsViewModel: DefaultItemsViewModel<Protocol>
    {
        private readonly Validator validator;
        private readonly WindowsService windowsService;
        protected override async Task<IEnumerable<Protocol>> OnLoadItems(AppContext appContext)
        {
            return appContext.Protocols.
                Include(x => x.People);
        }

        protected override Dictionary<string, string> Columns { get; set; } = new Dictionary<string, string>
        {
            {"Фамилия","People.Surname" },
            {"Имя","People.Name" },
            {"Отчество","People.Otchestvo" },
            {"Пол","People.Gender" },
            {"Национальность","People.Nationality" },
            {"Место рождения","People.BirthPlace" },
            {"Образование","People.Education" },
            {"Партийность","People.Party" },
            {"Семейное положение","People.Family" },
            {"Соц. полож. на момент ареста","Social" },
            {"Наказание","Punishment" },
            {"Постановление","Resolution" },
            {"Источник","Source" },
        };


        ProtocolWindow _window;

        public bool IsRemoveAll { get; set; }


        public ICommand SetupRemoveAll => new Command(x =>
        {
            IsRemoveAll = true;
            RemoveCommand?.Execute(null);
        });
        
        public ICommand SetupRemoveOnlyProtocol => new Command(x =>
        {
            IsRemoveAll = false;
            RemoveCommand?.Execute(null);
        });

        protected override void ItemSelected(Protocol item)
        {
            if (_window != null)
            {
                //OnEdit(item);
            }
        }


        protected override void OnAdd()
        {  
            ProtocolViewModel context = GetWindow();
            context.OnAdd();

            _window.Show();
        }

        private ProtocolViewModel GetWindow()
        {
            if (_window == null)
            {
                _window = new ProtocolWindow();
                _window.Closed += (a, b) => _window = null;

                var detailVm = ViewModelLocator.ServiceProvider.GetRequiredService<ProtocolViewModel>();
                detailVm.Accepted += DetailVm_Accepted;
                detailVm.Canceled += DetailVm_Canceled;
                detailVm.SetupValidator();
                detailVm.LoadItemsScources(appContext.Peoples.ToArray());
                _window.DataContext = detailVm;
                return detailVm;
            }
            else
            {
                _window.Focus();
                return _window.DataContext as ProtocolViewModel;
            }
        }

        private void DetailVm_Canceled(ProtocolViewModel obj)
        {
            _window?.Close();
        }

        private async void DetailVm_Accepted(ProtocolViewModel obj)
        {
            var copy = obj.Protocol.Clone() as Protocol;
            //await handler.Update(obj.Protocol);
            await handler.Update(copy);

            if (obj.IsEdit)
            {
                Items[indexOfEditable] = copy;
            }
            else
            {
                Items.Add(copy);
            }

            if (!obj.IsStayActive)
            {
                _window.Close();
            }
            //ItemsView.DeferRefresh();
            //RefreshCollectionView();            
        }

        int indexOfEditable;

        protected override void OnEdit(Protocol item)
        {  
            indexOfEditable = Items.IndexOf(item);
            var context = GetWindow();
            context.OnEdit(item);
            _window.Show();
        }

        protected override void OnRemove(Protocol[] items)
        {
            try
            {

                if (IsRemoveAll)
                {
                    if (Items.Any(x => items.SkipWhile(y => y.Id == x.Id).Any(y => y.PeopleId == x.PeopleId)) &&
                        MessageBox.Show("Удаляемый протокол ссылается на запись о человеке, которая, в свою очередь, есть в других протколах. После удаления этой записи, удалятся и все эти протоколы. Продолжить?",
                        "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        return;
                }

                handler.Remove(items, IsRemoveAll);
                Reload();
                //for (int i = 0; i < items.Length; i++)
                //{
                //    Items.Remove(items[i]);
                //}
                //RefreshCollectionView();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public ProtocolItemsViewModel(AppContext appContext,
                                      PageService pageService,
                                      Validator validator,
                                      WindowsService windowsService,
                                        IDataHandler<Protocol> protocolHandler) : base(appContext, pageService, protocolHandler)
        {
            this.validator = validator;
            this.windowsService = windowsService;
        }



        protected override ViewBase OnSetupView()
        {
            var grid = new GridView();

            grid.Columns.Add(new GridViewColumn() { Header = "Фамилия", DisplayMemberBinding = new Binding("People.Surname") });
            grid.Columns.Add(new GridViewColumn() { Header = "Имя", DisplayMemberBinding = new Binding("People.Name") });
            grid.Columns.Add(new GridViewColumn() { Header = "Отчество", DisplayMemberBinding = new Binding("People.Otchestvo") });
            grid.Columns.Add(new GridViewColumn() { Header = "Пол", DisplayMemberBinding = new Binding("People.Gender") { Converter = new Converters.ConverterGenderShort() } });
            grid.Columns.Add(new GridViewColumn() { Header = "Национальность", DisplayMemberBinding = new Binding("People.Nationality") });
            grid.Columns.Add(new GridViewColumn() { Header = "Место рождения", DisplayMemberBinding = new Binding("People.BirthPlace") });
            grid.Columns.Add(new GridViewColumn() { Header = "Дата рождения", DisplayMemberBinding = new Binding("People.BirthDate") });
            grid.Columns.Add(new GridViewColumn() { Header = "Образование", DisplayMemberBinding = new Binding("People.Education") });
            grid.Columns.Add(new GridViewColumn() { Header = "Партийность", DisplayMemberBinding = new Binding("People.Party")});
            grid.Columns.Add(new GridViewColumn() { Header = "Семейное положение", DisplayMemberBinding = new Binding("People.Family")});
            grid.Columns.Add(new GridViewColumn() { Header = "Соц. полож. на момент ареста", DisplayMemberBinding = new Binding("Social") });
            grid.Columns.Add(new GridViewColumn() { Header = "Номер протокола", DisplayMemberBinding = new Binding("ProtocolNumber") });
            grid.Columns.Add(new GridViewColumn() { Header = "Судебный орган", DisplayMemberBinding = new Binding("Organ") });
            grid.Columns.Add(new GridViewColumn() { Header = "Дата протокола", DisplayMemberBinding = new Binding("ProtocolDateTime") });
            grid.Columns.Add(new GridViewColumn() { Header = "Наказание", DisplayMemberBinding = new Binding("Punishment") });
            grid.Columns.Add(new GridViewColumn() { Header = "Постановление", DisplayMemberBinding = new Binding("Resolution") });
            grid.Columns.Add(new GridViewColumn() { Header = "Источник", DisplayMemberBinding = new Binding("Source") });


            return grid;
        }
    }
}
