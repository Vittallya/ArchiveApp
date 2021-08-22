using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArchiveApp.Resources.Components
{
    public class FixedVariantsFilterControl : FilterControl
    {
        private readonly Func<object, object, bool> func;

        public FixedVariantsFilterControl(
            FilterOption filterOption, 
            Array itemsSource,
            DependencyProperty toProperty,
            Func<object, object, bool> func,
            string displayMember) : base(filterOption)
        {
            ItemsSource = itemsSource;
            ToProperty = toProperty;
            this.func = func;
            DisplayMember = displayMember;

            Control = new ComboBox
            {
                DataContext = this,
                ItemsSource = ItemsSource,
                DisplayMemberPath = DisplayMember,
            };

            Control.SetBinding(ComboBox.SelectedItemProperty, new Binding(nameof(FilterValue)) 
            { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

        }

        public Array ItemsSource { get; }
        public DependencyProperty ToProperty { get; }
        public string DisplayMember { get; }



        public override bool OnFilter(object itemValue)
        {
            object filterItem = Control.GetValue(ToProperty);
            return func(itemValue, filterItem);
        }
    }
}
