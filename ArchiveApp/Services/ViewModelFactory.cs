using ArchiveApp.Abstract;
using ArchiveApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveApp.Services
{
    public class ViewModelFactory
    {
        Dictionary<Type, IDefaultItemsViewModel> _viewModels;

        Type _actualViewModelType = typeof(ProtocolItemsViewModel);

        public ViewModelFactory()
        {
            _viewModels = new Dictionary<Type, IDefaultItemsViewModel>();
        }


        public IDefaultItemsViewModel GetItemsViewModel(IServiceProvider provider)
        {
            if (_viewModels.ContainsKey(_actualViewModelType))
            {
                return _viewModels[_actualViewModelType];
            }
            var vm = provider.GetService(_actualViewModelType) as IDefaultItemsViewModel;

            _viewModels.Add(_actualViewModelType, vm);
            return vm;
        }

        public void SetupItemsViewModel<TViewModel>() where TViewModel : IDefaultItemsViewModel
        {
            _actualViewModelType = typeof(TViewModel);
        }

        public void SetupProtocolsViewModel()
        {
            _actualViewModelType = typeof(ProtocolItemsViewModel);
        }

    }
}
