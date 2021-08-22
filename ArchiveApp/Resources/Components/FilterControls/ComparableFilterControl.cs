using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ArchiveApp.Resources.Components
{
    public class ComparableFilterControl<TVal> : FilterControl
        where TVal: IComparable
    {
        private int selectedHelperIndex;
        private TVal compFilterValue;

        public TVal CompFilterValue 
        { 
            get => compFilterValue;
            set { compFilterValue = value; OnFilterValueChanged(); } 
        }

        public Array HelpingOptions { get; } = new string[]
        {"Меньше", "Равно", "Больше"};

        public int SelectedHelperIndex
        {
            get => selectedHelperIndex;
            set { selectedHelperIndex = value; OnFilterValueChanged(); }
        }

        public ComparableFilterControl(FilterOption filterOption) : base(filterOption)
        {
            IsHelpingOptions = true;
            FilterValue = new object();

            if (FilterOption.PropertyType == typeof(DateTime))
            {
                Control = new DatePicker() { DataContext = this };
                Control.SetBinding(DatePicker.SelectedDateProperty,
                    new Binding(nameof(CompFilterValue)) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                HelpingOptions = new string[] { "Раньше", "В этот момент", "Позже" };

            }
            else
            {
                Control = new TextBox { DataContext = this };
                Control.SetBinding(TextBox.TextProperty,
                    new Binding(nameof(CompFilterValue)) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
        }

        public ComparableFilterControl(FilterOption filterOption, TVal[] itemsSource) : base(filterOption)
        {
            IsHelpingOptions = true;
            FilterValue = new object();

            var itse = itemsSource;

            Control = new ComboBox { DataContext = this, ItemsSource = itse };
            Control.SetBinding(Selector.SelectedItemProperty,
                new Binding(nameof(CompFilterValue)) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            
        }




        public override bool OnFilter(object itemValue)
        {

            if (itemValue is IComparable valueComp)
            {
                int compare = SelectedHelperIndex - 1;
                int c = valueComp.CompareTo(CompFilterValue);

                if(c != 0)
                    c /= Math.Abs(c);

                return c == compare;
            }
            return false;
        }
    }
}
