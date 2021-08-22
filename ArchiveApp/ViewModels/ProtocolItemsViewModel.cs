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
using ArchiveApp.Resources;
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
                detailVm.LoadItemsScources(handler);
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
            Protocol copy = obj.Protocol;

            if (obj.IsStayActive)
            {
                copy = obj.Protocol.Clone() as Protocol;
            }

            if(!handler.UpdateProtocol(copy) || !await handler.SaveAsync())
            {
                MessageBox.Show(handler.Message);
                obj.IsStayActive = true;
                _window.Close();
                return;
            }

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

        protected override void OnSetupView(ColumnsBuilder<Protocol> builder)
        {



            var natio = dataService.GetUnits("natio");
            var ed = dataService.GetUnits("education");
            var party = dataService.GetUnits("party");
            var family = dataService.GetUnits("family");
            var soc = dataService.GetUnits("social");
            var org = dataService.GetUnits("organs");

            Func<object, object, bool> func = (item, filterItem) =>
            {
                if (filterItem is IComparable cFilter)
                {
                    return cFilter.CompareTo(item) == 0;
                    //todo при добавлении вспомогательных таблиц, здесь надо исправить
                }
                return false;
            };

            Func<object, object, bool> func1 = (value, filterValue) =>
            {
                if (value is IComparable cValue)
                {
                    return cValue.CompareTo(filterValue) == 0;
                }
                return false;
            };


            //builder.AddColumnWithDropDownListFilter(x => "People.Surname", "ФИО", peoples, TextBoxList.SelectedValueProperty, func1);


            builder.AddColumnWithDropDownListFilter(x => "People.Surname", "Фамилия", peoples, TextBoxList.SelectedValueProperty, func1);
            builder.AddColumnWithDropDownListFilter(x => "People.Name", "Имя", peoples, TextBoxList.SelectedValueProperty, func1);
            builder.AddColumnWithDropDownListFilter(x => "People.Otchestvo", "Отчество", peoples, TextBoxList.SelectedValueProperty, func1);
            builder.AddColumnWithFixedVariantsFilter(x => "People.Gender", "Пол", new string[] {"Мужской","Женский" }, default,
                (value, filter) =>
                {
                    if (value is bool bVal && filter is int index)
                    {
                        return bVal == !Convert.ToBoolean(index);
                    }

                    return false;
                }, ComboBox.SelectedIndexProperty, new Converters.ConverterGenderShort());

            builder.AddColumn(x => "People.BirthYear", "Год рождения", Enumerable.Range(1850, 100).Select(x => (short)x).ToArray());

            builder.AddColumnWithDropDownListFilter(x => "People.BirthPlace", "Место рождения", peoples, ComboBox.SelectedValueProperty, func, 
                displayMebmer: "Value");

            builder.AddColumnWithDropDownListFilter(x => "People.Natio.Name", "Национальность", natio, ComboBox.SelectedValueProperty, func, 
                displayMebmer: "Value");
            
            builder.AddColumnWithDropDownListFilter(x => "People.Education.Name", "Образование", ed, ComboBox.SelectedValueProperty, func, 
                displayMebmer: "Value");
            builder.AddColumnWithDropDownListFilter(x => "People.Party.Name", "Партийность", party, ComboBox.SelectedValueProperty, func, 
                displayMebmer: "Value");
            builder.AddColumnWithDropDownListFilter(x => "People.FamilyType.Name", "Семейное положение", family, ComboBox.SelectedValueProperty, func, 
                displayMebmer: "Value");

            builder.AddColumnWithDropDownListFilter(x => "Social.Name", "Кем работал на момент ареста", soc, ComboBox.SelectedValueProperty, func, displayMebmer: "Value");
            builder.AddColumnWithDropDownListFilter(x => nameof(x.ProtocolNumber), "По каким статьям УК РСФСР осужден", peoples, ComboBox.SelectedValueProperty, func1);
            builder.AddColumnWithDropDownListFilter(x => "Organ.Name", "Орган осуждения", org, ComboBox.SelectedValueProperty, func, displayMebmer: "Value");
            builder.AddColumnWithDropDownListFilter(x => nameof(x.Punishment), "Приговор", peoples, ComboBox.SelectedValueProperty, func1);
            builder.AddColumnWithDropDownListFilter(x => nameof(x.Resolution), "Постановление", peoples, ComboBox.SelectedValueProperty, func1);
            builder.AddColumnWithDropDownListFilter(x => nameof(x.Source), "Источник", peoples, ComboBox.SelectedValueProperty, func1);


        }
    }
}
