using System;
using System.Collections.Generic;
using ArchiveApp.Abstract;
using MVVM_Core;
using Models;
using BL;
using ArchiveApp.Services;
using BL.DbHandling;
using System.Threading.Tasks;
using ArchiveApp.Resources.Components;

namespace ArchiveApp.ViewModels
{
    class PeoplesViewModel : DefaultItemsViewModel<People>
    {
        public PeoplesViewModel(PageService pageService,
                                DropDownDataService dataService,
                                UnitOfWork unitOfWork): base(pageService, dataService, unitOfWork)
        {

        }

        protected async override Task<IEnumerable<People>> LoadItems()
        {
            return await handler.Peoples.LoadItemsAsync();
        }

        protected override void OnAdd()
        {
            throw new NotImplementedException();
        }

        protected override void OnEdit(People item)
        {
            throw new NotImplementedException();
        }

        protected override void OnRemove(People[] items)
        {
            throw new NotImplementedException();
        }

        protected override void OnSetupView(ColumnsBuilder<People> builder)
        {
            throw new NotImplementedException();
        }
    }
}
