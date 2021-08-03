using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;
using BL;
using BL.DbHandling;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace ArchiveApp.ViewModels
{
    [Singleton]
    public class MainViewModel: BaseViewModel
    {
        private readonly DbConnectionHandler db;
        private readonly PageService pageService;
        private readonly AppContextLoader loader;

        public MainViewModel(DbConnectionHandler db,
                             PageService pageService,
                             AppContextLoader loader)
        {
            this.db = db;
            this.pageService = pageService;
            this.loader = loader;
            pageService.PageChanged += PageService_PageChanged;
            Init();            
        }

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

        private async Task<bool> InitDb()
        {
            IsAnimVisible = true;
            Message = "Установка подключения к базе данных...";

            bool res = await db.TryConnect();

            if (!res)
            {
                Message = db.Message;
                return false;
            }

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

        private void Init()
        {
            pageService.ChangePage<Views.DefaultView>(DisappearAndToSlideAnim.Default);
        }
    }
}
