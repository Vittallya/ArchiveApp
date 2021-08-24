using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArchiveApp.Resources.Components
{
    public abstract class FilterControl
    {
        protected object filterValue;

        public FrameworkElement Control { get; set; }

        public FilterControl(FilterOption filterOption)
        {
            FilterOption = filterOption;
        }


        public bool IsClear
        {
            get
            {
                return FilterValue == null || FilterValue is string str && (str.Length == 0 || str == "*");
            }
        }

        public FilterOption FilterOption { get; }
        public bool IsHelpingOptions { get; set; }

        public object FilterValue
        {
            get => filterValue;
            set
            {
                filterValue = value;
                OnFilterValueChanged();
            }
        }

        protected void OnFilterValueChanged()
        {
            OnPrepare();
            FilterOption.OnFilterCountChanged();
        }

        protected virtual void OnPrepare() { }

        public abstract bool OnFilter(object itemValue);
    }




}
