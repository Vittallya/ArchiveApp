using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArchiveApp.Resources.Components
{
    public class ColumnComponent
    {
        public ColumnComponent(object header, string bindingProperty, IValueConverter converter = null, string stringFormat = null)
        {
            Header = header;
            BindingProperty = bindingProperty;
            Converter = converter;
            StringFormat = stringFormat;

            Column = new GridViewColumn()
            {
                Header = Header,
                DisplayMemberBinding = new Binding(BindingProperty) 
                { 
                    Converter = Converter, 
                    StringFormat = StringFormat 
                }

            };
            IsVisible = true;

        }

        public FilterOption FilterOption { get; }

        public ColumnComponent(object header, string bindingProperty, FilterOption filterOption, IValueConverter converter = null, string stringFormat = null)
            : this(header, bindingProperty, converter, stringFormat)
        {
            FilterOption = filterOption;
        }

        public bool IsVisible { get; set; }

        public object Header { get; }
        public string BindingProperty { get; }

        public IValueConverter Converter { get; }

        public string StringFormat { get; }


        public GridViewColumn Column { get; }
    }



    public class ColumnsBuilder<T>
    {
        private List<ColumnComponent> columns = new List<ColumnComponent>();

        public ColumnComponent[] GetColumnComponents()
        {
            return columns.ToArray();
        }

        public void AddColumn( Func<T, string> propGetter, string header,  bool isFilter = true,
            IValueConverter converter = null, string stringFormat = null)
        {
            string property = propGetter(default);

            ColumnComponent col;
            if (isFilter)
            {
                var filter = FilterOptionSource.GetFilter<T>(property, header);
                col = new ColumnComponent(header, property, filter, converter, stringFormat);
            }
            else
            {
                col = new ColumnComponent(header, property, converter, stringFormat);
            }

            columns.Add(col);
        }

        public void AddColumn<TVal>( Func<T, string> propGetter, string header,
            IValueConverter converter = null, string stringFormat = null)
            where TVal : IComparable
        {
            string property = propGetter(default);

            ColumnComponent col;
            var filter = FilterOptionSource.GetComparableFilter<T, TVal>(property, header);
            col = new ColumnComponent(header, property, filter, converter, stringFormat);

            columns.Add(col);
        }

        public void AddColumn<TVal>( Func<T, string> propGetter, string header, TVal[] items,
            IValueConverter converter = null, string stringFormat = null)
            where TVal : IComparable
        {
            string property = propGetter(default);

            ColumnComponent col;
            var filter = FilterOptionSource.GetComparableFilter<T, TVal>(property, header, items);
            col = new ColumnComponent(header, property, filter, converter, stringFormat);

            columns.Add(col);
        }


        public void AddColumn(Func<T, string> propGetter, string header,  FilterOption filter,
            IValueConverter converter = null, string stringFormat = null)
        {
            string property = propGetter(default(T));

            ColumnComponent col = new ColumnComponent(header, property, filter, converter, stringFormat);
            columns.Add(col);
        }

        public void AddColumnWithDropDownListFilter(Func<T, string> propGetter, string header, 
            Func<Array> itemsSourceGetter, DependencyProperty dProp, Func<object, object, object, bool> itemValueFunc,
            IValueConverter converter = null, string stringFormat = null, string displayMebmer = default, string valuePath = default)
        {
            string property = propGetter(default);

            if (displayMebmer == null)
            {
                if (property.Contains('.'))
                {
                    displayMebmer = property.Split('.')[^1];
                }
                else
                    displayMebmer = property;
            }
            if (valuePath == null)
            {
                valuePath = displayMebmer;
            }


            ColumnComponent col = new ColumnComponent(header, property, FilterOptionSource.GetStringVariantsOption<T>(property, header, itemsSourceGetter, displayMebmer, valuePath, dProp, itemValueFunc), converter, stringFormat);
            columns.Add(col);
        }

        public void AddColumnWithFixedVariantsFilter(Func<T, string> propGetter, string header, Array itemsSource, string displayMebmer,
            Func<object, object, bool> func,
            DependencyProperty toProp,
            IValueConverter converter = null, string stringFormat = null)
        {
            string property = propGetter(default(T));

            ColumnComponent col = 
                new ColumnComponent(header, property, FilterOptionSource.GetSelectionOption<T>(property, header, itemsSource, toProp, func, displayMebmer), converter, stringFormat);
            columns.Add(col);
        }
    }
}
