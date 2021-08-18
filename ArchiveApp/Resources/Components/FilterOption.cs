using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArchiveApp.Resources.Components
{

    public interface IFilterOption
    {
        public string Header { get; }
        public bool Filter(object item);
        public Type PropertyType { get; }

        public event Action<IFilterOption> FilterValueChanged;
        public ObservableCollection<FilterControl> FilterControls { get; }
    }

    public abstract class FilterOption : BaseViewModel, IFilterOption
    {
        private Func<object, object> valueGetter;
        private bool isAllConfition;

        public event Action<IFilterOption> FilterValueChanged;

        public ObservableCollection<FilterControl> FilterControls { get; }

        public string Property { get; }
        public string Header { get; }

        public void OnFilterChanged()
        {
            OnPrepare();
            FilterValueChanged?.Invoke(this);
        }

        protected virtual void OnPrepare() { }

        public bool IsHelpingOptions { get; set; }

        public FilterOption(string property, string header, Func<object, object> valueGetter, Type pType)
        {
            Property = property;
            Header = header;
            PropertyType = pType;
            this.valueGetter = valueGetter;
            FilterControls = new ObservableCollection<FilterControl>();
        }


        public Type PropertyType { get; }

        protected object GetValue(object item)
        {
            return valueGetter?.Invoke(item);
        }

        //public bool Filter(object item)
        //{
        //    if (FilterControls.All(y => y.IsClear))
        //        return true;
        //    object value = GetValue(item);
        //    return OnFilter(value);
        //}

        public bool IsAllConfition { 
            get => isAllConfition;
            set 
            {
                isAllConfition = value;
                OnFilterChanged();
            } 
        }

        public bool Filter(object item)
        {
            if (FilterControls.All(y => y.IsClear))
                return true;
            object value = GetValue(item);

            if (IsAllConfition)
            {
                return FilterControls.All(y => OnFilter(value, y));
            }

            return FilterControls.Any(y => OnFilter(value, y));
        }

        protected abstract bool OnFilter(object value, FilterControl control);
    }

    public class StringFilterOption: FilterOption
    {
        public Array ItemsSource { get; set; }

        public bool IsVariants { get; set; }
        public DependencyProperty ToProperty { get; internal set; }
        public string DisplayMember { get; internal set; }

        public StringFilterOption(string p, string h, Func<object, object> propertyInfo, Type pr) :base(p, h, propertyInfo, pr)
        {  }

        //protected override void OnPrepare()
        //{
        //    predicates = FilterControls.Where(x => x.FilterValue is string str && str.Length > 0).
        //    Select(x =>
        //    {
        //        string filterValue = x.FilterValue.ToString().ToLower();


        //        if (filterValue.Contains('*'))
        //        {
        //            if (filterValue.Length == 1)
        //            {
        //                Predicate<object> predicate = val => true;
        //                return predicate;
        //            }

        //            Predicate<object> predicate1 = (value) =>
        //            {
        //                if (value is string str)
        //                {
        //                    string[] parts = filterValue.Split('*');

        //                    string lower = str.ToLower();
        //                    int i = 0;
        //                    int lastIndex = -1;

        //                    return parts.All(y =>
        //                    {
        //                        if (i == parts.Length - 1 && i > 0)
        //                        {
        //                            if (y.Length == 0)
        //                            {
        //                                return lastIndex + 1 < lower.Length;
        //                            }
        //                            else
        //                            {
        //                                int last = lower.LastIndexOf(y);
        //                                return last + y.Length == lower.Length;
        //                            }
        //                        }

        //                        int index = lower.IndexOf(y);
        //                        bool res = index == 0 || (index > lastIndex && i > 0);
        //                        lastIndex = index;
        //                        i++;
        //                        return res;
        //                    });
        //                }
        //                return false;
        //            };

        //            return predicate1;

        //        }
        //        else
        //        {
        //            Predicate<object> predicate = v => filterValue.CompareTo(v.ToString().ToLower()) == 0;
        //            return predicate;
        //        }

        //    }).
        //    ToArray();
        //}

        protected override bool OnFilter(object value, FilterControl a)
        {
            if (value is string str && a.FilterValue is string filterValue)
            {
                filterValue = filterValue.ToLower();
                string lower = str.ToLower();

                if (filterValue.Contains('*'))
                {
                    if (filterValue.Length == 1)
                    {
                        return true;
                    }

                    string[] parts = filterValue.Split('*');

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
                };
                return filterValue.CompareTo(lower) == 0;
            }
            return false;
            //return IsAllConfition ? 
            //    predicates.All(p => p.Invoke(value)) : predicates.Any(p => p.Invoke(value));
        }
    }

    public class ComparableFilterOption : FilterOption
    {
        public ComparableFilterOption(string p, string h, Func<object, object> info, Type pr): base(p, h, info, pr)
        {
            IsHelpingOptions = true;
        }

        protected override bool OnFilter(object value, FilterControl a)
        {
            if (a.FilterValue.GetType() == value.GetType() &&
            a.FilterValue is IComparable filterValue && value is IComparable valueComp)
            {
                int compare = a.SelectedHelperIndex - 1;


                bool res = valueComp.CompareTo(filterValue) == compare;

                if (a.SelectedHelperIndex > 2)
                {
                    int compareOr = a.SelectedHelperIndex == 4 ? 1 : -1;
                    return res || valueComp.CompareTo(filterValue) == compareOr;
                }
                return res;
            }

            return false;
        }

    }

    public class SelectionFilerOption : FilterOption
    {
        public SelectionFilerOption(string property,
                                   string header,
                                   Func<object, object> valueGetter,
                                   Type pType) : base(property, header, valueGetter, pType)
        {
        }

        public string[] Selection { get; set; }

        public Func<object, object, bool> Predicate { get; set; }
        public DependencyProperty ToProperty { get; internal set; }

        protected override bool OnFilter(object value, FilterControl control)
        {
            return Predicate(value, control.FilterValue);
        }
    }

    public static class FilerOptionSource
    {
        public static FilterControl GetFilterControl(FilterOption option)
        {
            var filter = option;
            var pType = filter.PropertyType;

            var filterControl = new FilterControl(filter);

            if(option is SelectionFilerOption sel)
            {
                filterControl.Control = new ComboBox { ItemsSource = sel.Selection, DataContext = filterControl };
                filterControl.Control.SetBinding(sel.ToProperty, 
                    new Binding("FilterValue") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                return filterControl;
            }
            else if(option is StringFilterOption str && str.IsVariants)
            {
                filterControl.Control = new TextBoxList { 
                    ItemsSource = str.ItemsSource, 
                    DataContext = filterControl ,
                    DisplayMemberPath = str.DisplayMember,
                };
                //filterControl.Control.SetBinding(str.ToProperty,
                //    new Binding("FilterValue") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                
                filterControl.Control.SetBinding(TextBoxList.TextProperty,
                    new Binding("FilterValue") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                return filterControl;

            }

            if(pType.IsValueType &&
                pType.GetInterfaces().Any(x => x == typeof(IComparable)))
            {
                filterControl.IsHelpingOptions = true;
            }

            if (pType == typeof(DateTime))
            {
                filterControl.HelpingOptions = new string[] { "Раньше", "В этот момент", "Позже" };
                filterControl.Control = new DatePicker() { DataContext = filterControl };
                filterControl.Control.
                    SetBinding(DatePicker.SelectedDateProperty, new Binding("FilterValue") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
            else
            {
                filterControl.Control = new TextBox() { DataContext = filterControl };
                filterControl.Control.
                    SetBinding(TextBox.TextProperty, new Binding("FilterValue") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }


            return filterControl;
            //filter.FilterControls.Add(filterControl);
        }

        public static FilterOption GetSelectionOption<T>(string property, string header, string[] variants, 
            Func<DependencyProperty> bProp, Func<object, object, bool> predicate)
        {
            Type type = typeof(T);

            var info = GetPropertyInfo(ref property, ref type, out Func<object, object> valueGetter);

            var pType = info.PropertyType;


            SelectionFilerOption filter = new SelectionFilerOption(property, header, valueGetter, pType)
            {
                Selection = variants,
                Predicate = predicate,
                ToProperty = bProp(),
            };
            return filter;
        }
        public static FilterOption GetStringVariantsOption<T>(string property, 
            string header, 
            Array variants, string displayMember)
        {
            Type type = typeof(T);

            var info = GetPropertyInfo(ref property, ref type, out Func<object, object> valueGetter);

            var pType = info.PropertyType;


            StringFilterOption filter = new StringFilterOption(property, header, valueGetter, pType)
            {
                IsVariants = true,
                ItemsSource = variants,
                DisplayMember = displayMember,
            };
            return filter;
        }

        public static FilterOption GetFilter<T>(string property, string header)
        {
            Type type = typeof(T);

            var info = GetPropertyInfo(ref property, ref type, out Func<object, object> valueGetter);

            var pType = info.PropertyType;

            FilterOption filter = default;

            if (pType.GetInterfaces().Any(x => x == typeof(IComparable)))
            {
                if (pType == typeof(string))
                {
                    filter = new StringFilterOption(property, header, valueGetter, pType);
                }
                else
                {
                    filter = new ComparableFilterOption(property, header, valueGetter, pType);
                }
            }
            return filter;
        }

        private static PropertyInfo GetPropertyInfo(ref string property, ref Type type, out Func<object, object> valueGetter)
        {
            PropertyInfo info = default;

            if (property.Contains('.'))
            {
                List<PropertyInfo> subProps = new List<PropertyInfo>();

                string[] parts = property.Split('.');
                int i = 0;

                property = parts[parts.Length - 1];

                while (i < parts.Length - 1)
                {
                    string subProperty = parts[i];
                    var subInfo = type.GetProperty(subProperty);
                    subProps.Add(subInfo);

                    type = subInfo.PropertyType;
                    i++;
                }
                info = type.GetProperty(property);
                var arr = subProps.ToArray();

                valueGetter = item =>
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        item = arr[i].GetValue(item);
                    }
                    return info.GetValue(item);
                };
            }
            else
            {
                info = type.GetProperty(property);
                valueGetter = item => info.GetValue(item);
            }
            return info;
        }
    }
}


//bool inBegin = filterValue.LastIndexOf('*') == 0;
//bool inEnd = filterValue.IndexOf('*') == filterValue.Length - 1;


//if (inBegin || inEnd)
//{
//    bool EndsWith = str.EndsWith(filterValue.Substring(1, str.Length - 2));
//    bool StartsWith = str.StartsWith(filterValue.Substring(0, str.Length - 2));

//    return (inBegin && !inEnd && StartsWith) || (!inBegin && inEnd && EndsWith) || (EndsWith && StartsWith);
//}
//bool inBegin = filterValue.IndexOf('*') == 0;
//bool inEnd = filterValue.LastIndexOf('*') == filterValue.Length - 1;

//if (inBegin)
//    parts = parts.Skip(1).ToArray();
//if(inEnd)
//    parts = parts.SkipLast(1).ToArray();