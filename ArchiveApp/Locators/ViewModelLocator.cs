using ArchiveApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveApp.Locators
{
    public class ViewModelLocator
    {
        private static IServiceProvider _serviceProvider;

        public static void SetupServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MainViewModel MainViewModel => _serviceProvider.GetRequiredService<MainViewModel>();
        public DisplayGroupViewModel DisplayGroupViewModel => _serviceProvider.GetRequiredService<DisplayGroupViewModel>();
    }
}
