﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ArchiveApp.Resources.Components
{
    public class FilterControl
    {
        private object filterValue;
        private int selectedIndex;

        public FrameworkElement Control { get; set; }

        public FilterControl(FilterOption filterOption)
        {
            FilterOption = filterOption;
        }


        public bool IsClear
        {
            get
            {
                return FilterValue == null || FilterValue is string str && str.Length == 0;
            }
        }

        public FilterOption FilterOption { get; }

        public bool IsHelpingOptions { get; set; }
        public string[] HelpingOptions { get; set; } = new string[]
            {"Меньше", "Равно", "Больше", "Меньше или равно", "Больше или равно"};

        public int SelectedIndex 
        { 
            get => selectedIndex;
            set { selectedIndex = value; FilterOption.OnFilterChanged(); }       
        }

        public object FilterValue
        {
            get => filterValue;
            set
            {
                filterValue = value;
                FilterOption.OnFilterChanged();
            }
        }
    }
}