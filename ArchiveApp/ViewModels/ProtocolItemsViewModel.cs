using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ArchiveApp.Abstract;
using ArchiveApp.Locators;
using ArchiveApp.Resources;
using ArchiveApp.Resources.Components;
using ArchiveApp.Services;
using ArchiveApp.Views;
using BL;
using BL.DbHandling;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Models.Comparers;
using MVVM_Core;
using MVVM_Core.Validation;
using CustomControls;

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

        private async Task<bool> DetailVm_Accepted(ProtocolViewModel obj)
        {
            Protocol copy = obj.Protocol;

            if (obj.IsStayActive)
            {
                copy = obj.Protocol.Clone() as Protocol;
            }

            if(!handler.UpdateProtocol(copy) )
            {
                MessageBox.Show(handler.Message);
                obj.IsStayActive = true;
                _window.Close();
                return false;
            }
            SaveChangesResult res;
            if (!( res = await handler.SaveAsync()).Result)
            {
                if(res.ResultType == SaveChangesResultType.Error)
                {
                    MessageBox.Show(res.Exception.Message);
                    _window.Close();
                }
                else if(res.ResultType == SaveChangesResultType.NeedUpdate)
                {
                    MessageBox.Show("Для данной записи произошло обновление другим пользователем", "Система", MessageBoxButton.OK, MessageBoxImage.Information);

                    await handler.ReloadProtocol(copy);

                    Items[indexOfEditable] = copy.Clone() as Protocol;

                    //await Reload();
                    if (obj.IsStayActive)
                    {
                        obj.OnUpdate(copy);
                    }
                    else
                        _window.Close();
                }

                return false;
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
                return false;
            }
            return true;
        }

        int indexOfEditable;

        protected override void OnEdit(Protocol item)
        {  
            indexOfEditable = Items.IndexOf(item);
            var context = GetWindow();
            context.OnEdit(item);
            _window.Show();
        }

        protected override async void OnRemove(Protocol[] items)
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

                handler.RemoveProtocols(items, IsRemoveAll);
                await Reload();
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

            bool func(object item, object value, object filterItem)
            {
                if (value is IComparable cValue && filterItem is IComparable cFilter)
                {
                    return cValue.CompareTo(cFilter) == 0;
                    //todo при добавлении вспомогательных таблиц, здесь надо исправить
                }
                return false;
            }


            //builder.AddColumnWithDropDownListFilter(x => "People.Surname", "ФИО", peoples, TextBoxList.SelectedValueProperty, func1);

            IEnumerable<People> getterPeople() => handler.PeoplesClear;
            IEnumerable<Protocol> getterProtocol() => handler.ProtocolsClear;

            builder.AddColumnWithDropDownListFilter(x => "People.Surname", "Фамилия", () => getterPeople().Distinct(new GenericComparer<People>(x => x.Surname)).ToArray(), ComboBox.SelectedValueProperty, func);
            builder.AddColumnWithDropDownListFilter(x => "People.Name", "Имя", () => getterPeople().Distinct(new GenericComparer<People>(x => x.Name)).ToArray(), ComboBox.SelectedValueProperty, func);
            builder.AddColumnWithDropDownListFilter(x => "People.Otchestvo", "Отчество", () => getterPeople().Distinct(new GenericComparer<People>(x => x.Otchestvo)).ToArray(), ComboBox.SelectedValueProperty, func);
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

            builder.AddColumnWithDropDownListFilter(x => "People.BirthPlace", 
                "Место рождения",
                () => getterPeople().Distinct(new GenericComparer<People>(x => x.BirthPlace)).ToArray(),
                ComboBox.SelectedValueProperty,  func);

            builder.AddColumnWithDropDownListFilter(x => "People.Natio.Name", "Национальность", () => handler.Natios.Intersect(handler.Peoples.LoadItems(x => x.Natio).Select(x => x.Natio), new GenericComparer<Natio>(y => y.Id)).ToArray(),
                ComboBox.SelectedValueProperty, 
                (item, value, filter) => (item as Protocol)?.People?.NatioId.Value == short.Parse(filter.ToString()), valuePath: "Id");
            
            builder.AddColumnWithDropDownListFilter(x => "People.Education.Name", "Образование", () => handler.Educations.Intersect(handler.Peoples.LoadItems(x => x.Education).Select(x => x.Education), new GenericComparer<Education>(y => y.Id)).ToArray(),
                ComboBox.SelectedValueProperty,
                (item, value, filter) => (item as Protocol)?.People?.EducationId.Value == short.Parse(filter.ToString()), valuePath: "Id");
            builder.AddColumnWithDropDownListFilter(x => "People.Party.Name", "Партийность", () => handler.Parties.Intersect(handler.Peoples.LoadItems(y => y.Party).Select(y => y.Party), new GenericComparer<Party>(y => y.Id)).ToArray(),
                ComboBox.SelectedValueProperty, 
                (item, value, filter) => (item as Protocol)?.People?.PartyId.Value == short.Parse(filter.ToString()), valuePath: "Id");
            builder.AddColumnWithDropDownListFilter(x => "People.FamilyType.Name", "Семейное положение", 
                () => handler.FamilyTypes.Intersect(handler.Peoples.LoadItems(y => y.FamilyType).Select(y => y.FamilyType), new GenericComparer<FamilyType>(y => y.Id)).ToArray(), ComboBox.SelectedValueProperty, 
                (item, value, filter) => (item as Protocol)?.People?.FamilyTypeId.Value == short.Parse(filter.ToString()),valuePath: "Id");

            builder.AddColumnWithDropDownListFilter(x => "Social.Name", "Кем работал на момент ареста", () => handler.Socials.Intersect(handler.Protocols.LoadItems(y => y.Social).Select(y => y.Social), new GenericComparer<Social>(y => y.Id)).ToArray(),
                ComboBox.SelectedValueProperty, 
                (item, value, filter) => (item as Protocol)?.SocialId.Value == short.Parse(filter.ToString()),valuePath: "Id");

            //builder.AddColumnWithDropDownListFilter(x => nameof(x.ProtocolNumber), "По каким статьям УК РСФСР осужден", () => handler.Parties.ToArray(), ComboBox.SelectedValueProperty, func1);
            builder.AddColumnWithDropDownListFilter(x => "Organ.Name", "Орган осуждения", 
                () => handler.Organs.Intersect(handler.Protocols.LoadItems(y => y.Organ).Select(y => y.Organ), new GenericComparer<Organ>(y => y.Id)).ToArray(), 
                ComboBox.SelectedValueProperty, 
                (item, value, filter) => (item as Protocol)?.OrganId.Value == short.Parse(filter.ToString()), valuePath: "Id");

            builder.AddColumnWithDropDownListFilter(x => nameof(x.Punishment), "Приговор", () => getterProtocol().Distinct(new GenericComparer<Protocol>(p => p.Punishment)).ToArray(), ComboBox.SelectedValueProperty, func);
            builder.AddColumnWithDropDownListFilter(x => nameof(x.Resolution), "Постановление", () => getterProtocol().Distinct(new GenericComparer<Protocol>(p => p.Resolution)).ToArray(), ComboBox.SelectedValueProperty, func);
            builder.AddColumnWithDropDownListFilter(x => nameof(x.Source), "Источник", () => getterProtocol().Distinct(new GenericComparer<Protocol>(p => p.Source)).ToArray(), ComboBox.SelectedValueProperty, func);


        }
    }
}
