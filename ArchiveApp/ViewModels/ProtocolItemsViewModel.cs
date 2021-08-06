using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using ArchiveApp.Abstract;
using Microsoft.EntityFrameworkCore;
using Models;
using MVVM_Core;
using AppContext = Models.AppContext;

namespace ArchiveApp.ViewModels
{
    public class ProtocolItemsViewModel: DefaultItemsViewModel<Protocol>
    {
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
            grid.Columns.Add(new GridViewColumn() { Header = "Пол", DisplayMemberBinding = new Binding("People.Gender") });
            grid.Columns.Add(new GridViewColumn() { Header = "Национальность", DisplayMemberBinding = new Binding("People.Nationality.Name") });
            grid.Columns.Add(new GridViewColumn() { Header = "Место рождения", DisplayMemberBinding = new Binding("People.BirthPlace") });
            grid.Columns.Add(new GridViewColumn() { Header = "Образование", DisplayMemberBinding = new Binding("People.Education") });
            grid.Columns.Add(new GridViewColumn() { Header = "Партийность", DisplayMemberBinding = new Binding("People.Party") });
            grid.Columns.Add(new GridViewColumn() { Header = "Семейное положение", DisplayMemberBinding = new Binding("People.Family") });
            grid.Columns.Add(new GridViewColumn() { Header = "Соц. полож. на момент ареста", DisplayMemberBinding = new Binding("Social") });
            grid.Columns.Add(new GridViewColumn() { Header = "Наказание", DisplayMemberBinding = new Binding("Punishment") });
            grid.Columns.Add(new GridViewColumn() { Header = "Постановление", DisplayMemberBinding = new Binding("Resolution") });
            grid.Columns.Add(new GridViewColumn() { Header = "Источник", DisplayMemberBinding = new Binding("Source") });


            return grid;
        }

        public ProtocolItemsViewModel(AppContext appContext, PageService pageService): base(appContext, pageService)
        {
        }
    }
}
