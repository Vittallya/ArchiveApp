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
            ConnectionStatus = ConnectionStatus.Connecting;

            Message = "Установка подключения к базе данных...";

            await loader.TryLoad(IsCreateDb);

            if (!loader.Result)
            {
                Message = loader.Message;
            }

            return loader.Result;
        } 
        private async Task InitFiles()
        {
            Message = "Загрузка данных..";

            if (!fileService.IsFileExist)
            {
                fileService.CreateFile();                
            }
            await dataService.ReloadData();
        }
        private async void Init()
        {
            await InitAsync();
        }


        private async Task InitAsync()
        {
            IsAnimVisible = true;
            Message = "Пизда!";
            if (!await InitDb())
            {
                ConnectionStatus = ConnectionStatus.Error;
                return;
            }
            ConnectionStatus = ConnectionStatus.Connected;

            await InitFiles();

            SetupViewModel<ProtocolItemsViewModel>();
            Message = "Успешно!";
            IsTextVisible = false;
            IsAnimVisible = false;

        }

        public bool IsError => ConnectionStatus == ConnectionStatus.Error;

        public ICommand ReConnect => new CommandAsync(async y => { IsCreateDb = true; await InitAsync(); });

        public bool IsCreateDb { get; set; }

        public string StatusStr => StatusWords[ConnectionStatus];

        public Dictionary<ConnectionStatus, string> StatusWords { get; } = new Dictionary<ConnectionStatus, string>
        {
            {ConnectionStatus.Connected, "Подключено" },
            {ConnectionStatus.NotConnected, "Не подключено" },
            {ConnectionStatus.Error, "Ошибка" },
            {ConnectionStatus.Connecting, "Подключение" },
        };

        public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.NotConnected;

        public bool IsCreate { get; set; }

        public Dictionary<string, ICommand> Tables { get; }
    }



    public enum ConnectionStatus
    {
        Connected, NotConnected, Error, Connecting
    }
}
