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

        public event Action<int> FilterCountChanged;
        public FilterOption[] FilterOptions { get; private set; }
        public Dictionary<short, FilterOption> NumeredFilterOptions { get; private set; } = new Dictionary<short, FilterOption>();


        public void SetupFilterOptions(FilterOption[] options)
        {
            FilterOptions = options;

            short leng = (short)FilterOptions.Length;

            NumeredFilterOptions.Clear();

            for (short i = (short)(leng - 1); i >= 0; i--)
            {
                NumeredFilterOptions.Add(i, FilterOptions[leng - i - 1]);
            }

        }


        private void OnFilterCountChanged()
        {
            int count = FilterOptions.Sum(f => f.FilterControls.Count);
            FilterCountChanged?.Invoke(count);
        }

        public ICommand AddFilterControl => addFilterControl ??= new Command(x =>
        {
            if(x is FilterOption option)
            {
                option.FilterControls.Add(option.GetFilterControl());
            }
            OnFilterCountChanged();
        });

        public ICommand RemoveFilterControl => removeFilterControl ??= new Command(x =>
        {
            if(x is FilterControl control)
            {
                control.FilterOption.FilterControls.Remove(control);
                control.FilterOption.OnFilterChanged(!control.IsClear);
            }
            OnFilterCountChanged();
        });

        public ICommand ClearAllFilterControls => clearAllFilterControls ??= new Command(x =>
        {
            if (x is FilterOption option)
            {
                option.FilterControls.Clear();
                option.OnFilterChanged();
            }
            else
            {
                for (int i = 0; i < FilterOptions.Length; i++)
                {
                    var opt = FilterOptions[i];
                    opt.FilterControls.Clear();
                }
            }
            if (FilterOptions.Length > 0)
            {
                FilterOptions[0].OnFilterChanged();
            }
            OnFilterCountChanged();
        });
    }
}
