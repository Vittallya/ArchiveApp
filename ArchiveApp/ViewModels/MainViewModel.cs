using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;
using BL;
using BL.DbHandling;
using System.Windows.Controls;
using System.Threading.Tasks;
using ArchiveApp.Abstract;
using ArchiveApp.Locators;
using ArchiveApp.Services;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace ArchiveApp.ViewModels
{
    [Singleton]
    public class MainViewModel: BaseViewModel
    {
        private readonly DbConnectionHandler db;
        private readonly PageService pageService;
        private readonly ViewModelFactory factory;
        private readonly XmlFileService fileService;
        private readonly DropDownDataService dataService;
        private readonly AppContextLoader loader;

        public MainViewModel(DbConnectionHandler db,
                             PageService pageService,
                             ViewModelFactory factory,
                             XmlFileService fileService,
                             DropDownDataService dataService,
                             AppContextLoader loader)
        {
            this.db = db;
            this.pageService = pageService;
            this.factory = factory;
            this.fileService = fileService;
            this.dataService = dataService;
            this.loader = loader;
            pageService.PageChanged += PageService_PageChanged;

            Tables = new Dictionary<string, ICommand>
            {
                {"Протоколы", new Command(x => SetupViewModel<ProtocolItemsViewModel>()) }
            };

            Init();            
        }


        private void SetupViewModel<TViewModel>() where TViewModel: IDefaultItemsViewModel
        {
            factory.SetupItemsViewModel<TViewModel>();
            ItemsViewModel = ViewModelLocator.ServiceProvider.GetRequiredService<IDefaultItemsViewModel>();
            ItemsViewModel.ChangePage();
        }

        public IDefaultItemsViewModel ItemsViewModel { get; private set; }

        public Page CurrentPage { get; private set; }

        private async void PageService_PageChanged(System.Windows.Controls.Page page, ISliderAnimation anim)
        {
            Page oldPage = CurrentPage;

            if(anim != null && oldPage != null)
                await anim.AnimateOldPage(oldPage);

            CurrentPage = page;
            if(anim != null)
                await anim.AnimateNewPage(CurrentPage);
        }

        public string Message { get; private set; }

        public bool IsAnimVisible { get; private set; }
        public bool IsTextVisible { get; private set; } = true;

        public bool IsSidePanelVisible { get; set; }

        private async Task<bool> InitDb()
        {
            IsAnimVisible = true;
            Message = "Установка подключения к базе данных...";

            await loader.TryLoad();

            if (!loader.Result)
            {
                Message = loader.Message;
                return false;
            }

            Message = "Подключение успешно!";
            IsTextVisible = false;

            return true;
        } 
        private void InitFiles()
        {
            if(!fileService.IsFileExist)
            {
                fileService.CreateFile();                
            }
            dataService.ReloadData();
        }
        private async void Init()
        {
            await InitDb();
            InitFiles();
        }

        public Dictionary<string, ICommand> Tables { get; }
    }
}
