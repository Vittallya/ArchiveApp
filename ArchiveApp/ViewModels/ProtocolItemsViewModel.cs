using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using ArchiveApp.Abstract;
using ArchiveApp.Locators;
using ArchiveApp.Resources.Components;
using ArchiveApp.Views;
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

        protected override async Task OnLoadItems(AppContext appContext)
        {
            await appContext.Protocols.
                Include(x => x.Organ).
                Include(x => x.People).ThenInclude(y => y.Nationality).
                LoadAsync();
        }

        protected override ViewBase OnSetupView()
        {
            var grid = new GridView();

            grid.Columns.Add(new GridViewColumn() { Header = "Фамилия", DisplayMemberBinding = new Binding("People.Surname") });
            grid.Columns.Add(new GridViewColumn() { Header = "Имя", DisplayMemberBinding = new Binding("People.Name") });
            grid.Columns.Add(new GridViewColumn() { Header = "Отчество", DisplayMemberBinding = new Binding("People.Otchestvo") });
            grid.Columns.Add(new GridViewColumn() { Header = "Пол", DisplayMemberBinding = new Binding("People.Gender") {Converter = new Converters.ConverterGenderShort() } });
            grid.Columns.Add(new GridViewColumn() { Header = "Национальность", DisplayMemberBinding = new Binding("People.Nationality.Name") });
            grid.Columns.Add(new GridViewColumn() { Header = "Место рождения", DisplayMemberBinding = new Binding("People.BirthPlace") });
            grid.Columns.Add(new GridViewColumn() { Header = "Образование", DisplayMemberBinding = new Binding("People.Education") { Converter = new Converters.ConverterEducation() } });
            grid.Columns.Add(new GridViewColumn() { Header = "Партийность", DisplayMemberBinding = new Binding("People.Party") { Converter = new Converters.ConverterParty() } });
            grid.Columns.Add(new GridViewColumn() { Header = "Семейное положение", DisplayMemberBinding = new Binding("People.Family") { Converter = new Converters.ConverterFamily() } });
            grid.Columns.Add(new GridViewColumn() { Header = "Соц. полож. на момент ареста", DisplayMemberBinding = new Binding("Social") { Converter = new Converters.ConverterSocial() } });
            grid.Columns.Add(new GridViewColumn() { Header = "Наказание", DisplayMemberBinding = new Binding("Punishment") });
            grid.Columns.Add(new GridViewColumn() { Header = "Постановление", DisplayMemberBinding = new Binding("Resolution") });
            grid.Columns.Add(new GridViewColumn() { Header = "Источник", DisplayMemberBinding = new Binding("Source") });


            return grid;
        }

        ProtocolWindow _window;

        protected override async Task OnAdd()
        {
            if (_window == null)
            {
                _window = new ProtocolWindow();
                var context = SetupDetailContext();
                _window.DataContext = context;

                context.SelectedPeople = new People() {BirthDate = new DateTime(1920, 10, 10) };
                context.SelectedProtocol = new Protocol() { ProtocolDateTime = new DateTime(1937, 10, 10) };
                context.AcceptCommand = new Command(Accepted);
                
                _window.Show();
            }
            else
            {
                _window.Focus();
            }

        }


        private void Accepted(object param)
        {
            var vm = _window?.DataContext as ProtocolViewModel;

            if(vm != null)
            {
                if (vm.IsCorrect)
                {
                    
                    if(vm.SelectedOrgan == null)
                    {
                        vm.SelectedProtocol.Organ = new Organ { Id = default, Name = vm.OrganText };
                    }
                    else
                    {
                        vm.SelectedProtocol.OrganId = vm.SelectedOrgan.Id;
                    }

                    if(vm.SelectedNationality == null)
                    {
                        vm.SelectedPeople.Nationality = new Nationality { Id = default, Name = vm.NationalityText };
                    }
                    else
                    {
                        vm.SelectedPeople.NationalityId = vm.SelectedNationality.Id;
                    }

                    vm.SelectedProtocol.People = vm.SelectedPeople;
                    string[] split = vm.FIO.Split(' ');

                    vm.SelectedPeople.Surname = split[0];
                    vm.SelectedPeople.Name = split[1];
                    vm.SelectedPeople.Otchestvo = split[2];

                    appContext.Protocols.Add(vm.SelectedProtocol);
                    //Протокол

                    appContext.SaveChanges();
                    Items.Add(vm.SelectedProtocol);


                    if (!vm.IsStayActive)
                    {
                        _window.Close();
                        _window = null;
                    }
                    else
                    {
                        if(vm.SelectedNationality == null)
                        {
                            var list = vm.AllNationalities.ToList();
                            list.Add(vm.SelectedPeople.Nationality);
                            vm.AllNationalities = list.ToArray();
                        }
                        if(vm.SelectedOrgan == null)
                        {
                            var list = vm.AllOrgans.ToList();
                            list.Add(vm.SelectedProtocol.Organ);
                            vm.AllOrgans = list.ToArray();
                        }
                        vm.SelectedProtocol = new Protocol
                        {
                            OrganId = vm.SelectedProtocol.OrganId,
                            PeopleId = vm.SelectedProtocol.PeopleId,
                            ProtocolDateTime = vm.SelectedProtocol.ProtocolDateTime,
                            ProtocolNumber = vm.SelectedProtocol.ProtocolNumber,
                            Punishment = vm.SelectedProtocol.Punishment,
                            Resolution = vm.SelectedProtocol.Resolution,
                            Social = vm.SelectedProtocol.Social,
                            Source = vm.SelectedProtocol.Source
                        };
                    }
                }
            }

        }


        private ProtocolViewModel SetupDetailContext()
        {
            var detailVm = ViewModelLocator.ServiceProvider.GetRequiredService<ProtocolViewModel>();
            detailVm.AllOrgans = appContext.Organs.Local.ToArray();
            detailVm.AllNationalities = appContext.Nationalities.Local.ToArray();
            detailVm.AllPeoples = appContext.
                Peoples.
                Select(x => new PeopleFio { Id = x.Id, Fio = $"{x.Surname} {x.Name} {x.Otchestvo}" }).ToArray();

            detailVm.SetupValidator();
            return detailVm;
        }



        protected override async Task OnEdit(Protocol item)
        {
            
        }

        protected override async Task OnRemove(Protocol[] items)
        {
        }

        public ProtocolItemsViewModel(AppContext appContext,
                                      PageService pageService,
                                      Validator validator,
                                      WindowsService windowsService) : base(appContext, pageService)
        {
            this.validator = validator;
            this.windowsService = windowsService;
        }
    }
}
