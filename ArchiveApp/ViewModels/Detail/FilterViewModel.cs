using ArchiveApp.Resources.Components;
using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ArchiveApp.ViewModels
{
    public class FiltersViewModel : BaseViewModel
    {
        private ICommand addFilterControl;
        private ICommand removeFilterControl;
        private ICommand clearAllFilterControls;
        
        public FilterOption[] FilterOptions { get; set; }

        public ICommand AddFilterControl => addFilterControl ??= new Command(x =>
        {
            if(x is FilterOption option)
            {
                option.FilterControls.Add(FilerOptionSource.GetFilterControl(option));
            }
        });

        public ICommand RemoveFilterControl => removeFilterControl ??= new Command(x =>
        {
            if(x is FilterControl control)
            {
                control.FilterOption.FilterControls.Remove(control);
                if(!control.IsClear)
                    control.FilterOption.OnFilterChanged();   
            }
        });

        public ICommand ClearAllFilterControls => clearAllFilterControls ??= new Command(x =>
        {
            bool isUpdate = false;


            if (x is FilterOption option)
            {
                isUpdate = option.FilterControls.Any(c => !c.IsClear);
                option.FilterControls.Clear();
            }
            else
            {
                isUpdate = FilterOptions.Any(y => y.FilterControls.Any(z => !z.IsClear));
                for (int i = 0; i < FilterOptions.Length; i++)
                {
                    var opt = FilterOptions[i];
                    opt.FilterControls.Clear();
                }
            }
            if (isUpdate && FilterOptions.Length > 0)
            {
                FilterOptions[0].OnFilterChanged();
            }

        });
    }
}
