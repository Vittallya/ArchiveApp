using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArchiveApp.Resources.Components
{
    public class StringFilterControl : FilterControl
    {
        private int selectedHelperIndex;

        public bool IsDropDownList { get; }

        public StringFilterControl(FilterOption filterOption) : base(filterOption)
        {
            Control = new TextBox() { DataContext = this };
            Control.SetBinding(TextBox.TextProperty, new Binding(nameof(FilterValue)) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        }

        public StringFilterControl(FilterOption filterOption,
            Array itemsSource, string displayMember, string valuePath, DependencyProperty toProperty, Func<object, object, bool> func) : this(filterOption)
        {
            IsDropDownList = true;
            ItemsSource = itemsSource;
            DisplayMember = displayMember;
            ValuePath = valuePath;
            ToProperty = toProperty;
            this.func = func;
            Control = new TextBoxList
            {
                ItemsSource = ItemsSource,
                DisplayMemberPath = DisplayMember,
                SelectedValuePath = valuePath,
                DataContext = this,
            };

            Control.SetBinding(TextBoxList.TextProperty, new Binding("FilterValue") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //Control.SetBinding(TextBoxList.SelectedItemProperty, new Binding("SelectedItem") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        }

        public override bool OnFilter(object itemValue)
        {
            return predicate(itemValue);
        }

        Predicate<object> predicate;
        private readonly Func<object, object, bool> func;

        protected override void OnPrepare()
        {
            //if (SelectedItem != null)
            //    predicate = value => func(value, Control.GetValue(ToProperty));

            var obj = Control.GetValue(ToProperty);

            if(func != null && obj != null)
            {
                predicate = value => func(FilterOption.OriginItem, obj);
            }
            else if (FilterValue is string filterValue)
            {
                filterValue = filterValue.ToLower();

                if (filterValue.Contains('*'))
                {
                    string[] parts = filterValue.Split('*');

                    predicate = value =>
                    {
                        if (value is string str)
                        {
                            string lower = str.ToLower();

                            int i = 0;
                            int lastIndex = -1;

                            return parts.All(y =>
                            {
                                if (i == parts.Length - 1 && i > 0)
                                {
                                    if (y.Length == 0)
                                    {
                                        return lastIndex + 1 < lower.Length;
                                    }
                                    else
                                    {
                                        int last = lower.LastIndexOf(y);
                                        return last + y.Length == lower.Length;
                                    }
                                }

                                int index = lower.IndexOf(y);
                                bool res = index == 0 || (index > lastIndex && i > 0);
                                lastIndex = index;
                                i++;
                                return res;
                            });
                        }
                        return false;
                    };
                }
                else
                {
                    predicate = value => filterValue.CompareTo(value?.ToString()?.ToLower()) == 0;
                }
            }
            else
            {
                predicate = value => false;
            }
        }

        public int SelectedHelperIndex
        {
            get => selectedHelperIndex;
            set { selectedHelperIndex = value; OnFilterValueChanged(); }
        }
        public Array ItemsSource { get; }
        public string DisplayMember { get; }
        public string ValuePath { get; }
        public DependencyProperty ToProperty { get; }
    }
}
