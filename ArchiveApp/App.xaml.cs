using ArchiveApp.Locators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ArchiveApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MVVM_Core.ServiceProviderBuilder builder = new MVVM_Core.ServiceProviderBuilder();
            builder.IncludeViewModels();
            builder.IncludeBaseServices();
            builder.UseStartup<Startup>();
            builder.UseStartup<BL.Startup>();

            ViewModelLocator.SetupServiceProvider(builder.BuidSeriveProvider());
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            base.OnStartup(e);
        }
    }
}
