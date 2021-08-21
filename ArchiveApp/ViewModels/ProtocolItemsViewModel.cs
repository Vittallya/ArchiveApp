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
using ArchiveApp.Services;
using ArchiveApp.Views;
using BL.Abstract;
using BL.DbHandling;
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

        private People[] peoples;

        protected override void SetupFilterFields(List<FilterOption> list)
        {
            var natio = dataService.GetUnits("natio");
            var ed = dataService.GetUnits("education");
            var party = dataService.GetUnits("party");
            var family = dataService.GetUnits("family");
            var soc = dataService.GetUnits("social");
            var org = dataService.GetUnits("organs");


            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("People.Surname", "Фамилия", peoples, "Surname"));
            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("People.Name", "Имя", peoples, "Name"));
            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("People.Otchestvo", "Отчество", peoples, "Otchestvo"));

            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("People.Nationality", "Национальность", natio, "Value"));
            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("People.Education", "Образование", ed, "Value"));
            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("People.Party", "Партийность", party, "Value"));
            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("People.Family", "Семейное положение", family, "Value"));
            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("Social", "Соц. полож. на момент ареста", soc, "Value"));
            list.Add(FilerOptionSource.GetStringVariantsOption<Protocol>("Organ", "Судебный орган", org, "Value"));
            list.Add(FilerOptionSource.GetSelectionOption<Protocol>("People.Gender", "Пол", new string[] {"Мужской","Женский" }, 
                () => ComboBox.SelectedIndexProperty, 
                (value, filter) =>
                {
                    if(value is bool bVal && filter is int index)
                    {
                        return bVal == !Convert.ToBoolean(index);
                    }

                    return false;
                }));
        }

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
                detailVm.LoadItemsScources(peoples);
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

        private async Task DetailVm_Accepted(ProtocolViewModel obj)
        {
            var copy = obj.Protocol.Clone() as Protocol;
            if(!handler.Protocols.Update(copy))
            {
                MessageBox.Show(handler.Protocols.Message);
                return;
            }
            await handler.SaveAsync();

            hasChanges = true;

            if (obj.IsEdit)
            {
                Items[indexOfEditable] = copy;
                SetupNonNotifyProperty(nameof(SelectedItem));
                SelectedItem = copy;
            }
            else
            {
                Items.Add(copy);
            }

            if (!obj.IsStayActive)
            {
                _window.Close();
            }       
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

                handler.Protocols.Remove(items, IsRemoveAll);
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

        public ProtocolItemsViewModel(PageService pageService,
                                      Validator validator,
                                      DropDownDataService dataService, 
                                      WindowsService windowsService,
                                        UnitOfWork protocolHandler) : base(pageService, dataService, protocolHandler)
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
            grid.Columns.Add(new GridViewColumn() { Header = "Год рождения", DisplayMemberBinding = new Binding("People.BirthYear")});
            grid.Columns.Add(new GridViewColumn() { Header = "Образование", DisplayMemberBinding = new Binding("People.Education") });
            grid.Columns.Add(new GridViewColumn() { Header = "Партийность", DisplayMemberBinding = new Binding("People.Party")});
            grid.Columns.Add(new GridViewColumn() { Header = "Семейное положение", DisplayMemberBinding = new Binding("People.Family")});
            grid.Columns.Add(new GridViewColumn() { Header = "Кем работал на момент ареста", DisplayMemberBinding = new Binding("Social") });
            grid.Columns.Add(new GridViewColumn() { Header = "По каким статьям УК РСФСР осужден", DisplayMemberBinding = new Binding("ProtocolNumber") });
            grid.Columns.Add(new GridViewColumn() { Header = "Судебный орган", DisplayMemberBinding = new Binding("Organ") });
            grid.Columns.Add(new GridViewColumn() { Header = "Год осуждения", DisplayMemberBinding = new Binding("ProtocolYear") });
            grid.Columns.Add(new GridViewColumn() { Header = "Приговор", DisplayMemberBinding = new Binding("Punishment") });
            grid.Columns.Add(new GridViewColumn() { Header = "Постановление", DisplayMemberBinding = new Binding("Resolution") });
            grid.Columns.Add(new GridViewColumn() { Header = "Источник", DisplayMemberBinding = new Binding("Source") });


            return grid;
        }

        bool hasChanges = true;

        protected override async Task<IEnumerable<Protocol>> LoadItems()
        {
            var a = await handler.Protocols.LoadItemsAsync();
            if (hasChanges)
            {
                peoples = a.Select(x => x.People).ToArray();
            }

            return a;
        }
    }
}
