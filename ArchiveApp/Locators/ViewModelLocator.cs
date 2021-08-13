using ArchiveApp.Abstract;
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

        public static IServiceProvider ServiceProvider => _serviceProvider;

        public static void SetupServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MainViewModel MainViewModel => _serviceProvider.GetRequiredService<MainViewModel>();
        public IDefaultItemsViewModel ItemsViewModel => _serviceProvider.GetRequiredService<IDefaultItemsViewModel>();

    }
}
