using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArchiveApp.Resources
{
    /// <summary>
    /// Логика взаимодействия для TextBoxList.xaml
    /// </summary>
    public partial class TextBoxList : UserControl
    {
        #region D Props
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(TextBoxList),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChagedStatic));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(IEnumerable), typeof(TextBoxList),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnItemsSourceChangedStatic));


        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            "SelectedIndex", typeof(int), typeof(TextBoxList), 
            new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChangedStatic));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(object), typeof(TextBoxList), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChangedStatic));

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
            "SelectedValue", typeof(object), typeof(TextBoxList), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChangedStatic));

        public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register(
            "SelectedValuePath", typeof(string), typeof(TextBoxList), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValuePathChangedStatic));

        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
            "DisplayMemberPath", typeof(string), typeof(TextBoxList),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayMemberChangedStatic));

        public static readonly DependencyProperty IsSearchEnabledProperty = DependencyProperty.Register(
            "IsSearchEnabled", typeof(bool), typeof(TextBoxList),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsSearchEnabledChangedStatic));
        #endregion 
        #region Props
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public string DisplayMemberPath
        {
            get => GetValue(DisplayMemberPathProperty)?.ToString();
            set => SetValue(DisplayMemberPathProperty, value);
        }
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }
        public string SelectedValuePath
        {
            get => GetValue(SelectedValuePathProperty)?.ToString();
            set => SetValue(SelectedValuePathProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public bool IsSearchEnabled
        {
            get => (bool)GetValue(IsSearchEnabledProperty);
            set => SetValue(IsSearchEnabledProperty, value);
        }
        #endregion

        #region OnChanged

        bool first = true;

        private void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (first)
            {
                first = false;
                return;
            }
            OnFocused();
        }

        private void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null)
            {
                Type srcType = ItemsSource.GetType();
                itemType = srcType.GetElementType();

                if(itemType == null && srcType.IsGenericType)
                {
                    itemType = srcType.GetGenericArguments()[0];
                }

                SetupDisplayPath();

                if (SelectedValuePath != null)
                {
                    SetupProperty(itemType, SelectedValuePath, ref valueProperty);
                }
            }

        }

        private void SetupDisplayPath()
        {
            if (DisplayMemberPath != null && itemType != null)
            {
                SetupProperty(itemType, DisplayMemberPath, ref displayProperty);
                displaySource = ItemsSource.Cast<object>()?.Select(x =>
                {
                    string display = displayProperty?.GetValue(x)?.ToString();
                    return new DisplayItem(display, x);

                })?.ToArray();
            }
            else
            {
                displaySource = ItemsSource.Cast<object>().Select(x =>
                {
                    return new DisplayItem(x.ToString(), x);

                }).ToArray();
            }
        }

        private void OnDisplayMebmerPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue == null)
            {
                displayProperty = null;
            }
            else if (itemType != null)
            {
                SetupProperty(itemType, DisplayMemberPath, ref displayProperty);
                SetupDisplayPath();
            }
        }

        private void OnSelectedValuePathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                valueProperty = null;
            }
            else if(itemType != null)
            {
                SetupProperty(itemType, SelectedValuePath, ref valueProperty);
            }
        }
        private void OnSelectedItemChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue == null)
            {
                SelectedValue = null;
                //tb.Text = null;
            }
            else
            {
                isPaste = true;

                Text = displayProperty != null ? 
                    displayProperty.GetValue(e.NewValue)?.ToString() : e.NewValue?.ToString();
                
                SelectedValue = valueProperty != null ?
                    valueProperty.GetValue(e.NewValue) : e.NewValue;
            }

        }
        private void OnSelectedValueChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (valueProperty != null)
            {
                isPaste = true;

                if(e.NewValue != null)
                {
                    if (SelectedItem != null)
                    {
                        Text = displayProperty != null ?
                            displayProperty.GetValue(SelectedItem)?.ToString() : SelectedItem?.ToString();
                    }
                    else
                    {

                        var obj = displaySource.FirstOrDefault(x => valueProperty.GetValue(x.Item)?.Equals(e.NewValue) ?? false).Item;
                        if (obj != null)
                        {
                            Text = displayProperty != null ?
                                displayProperty.GetValue(obj)?.ToString() : obj?.ToString();
                        }
                    }

                }
                else if (DisplayMemberPath == SelectedValuePath)
                {
                    Text = null;
                }

            }
        }

        private void OnIsSearchEnabledChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool res)
            {
                if (!res)
                    toggle.IsChecked = false;

                toggle.Visibility = res ? Visibility.Visible : Visibility.Collapsed;
                listView.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        private void SetupProperty(Type itemType, string path, ref PropertyInfo prop)
        {
            prop = itemType.GetProperty(path);
        }


        DisplayItem[] displaySource;

        PropertyInfo valueProperty;
        PropertyInfo displayProperty;
        Type itemType;

        public TextBoxList()
        {
            InitializeComponent();
        }

        bool isPaste;

        private void tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (listView.SelectedIndex + 1 < listView.Items.Count)
                    listView.SelectedIndex++;
                else
                    listView.SelectedIndex = 0;
            }

            else if (e.Key == Key.Up)
            {
                if (listView.SelectedIndex > 0)
                    listView.SelectedIndex--;
                else
                    listView.SelectedIndex = listView.Items.Count - 1;
            }
            else if ((e.Key == Key.Enter || e.Key == Key.Tab) && listView.Visibility == Visibility.Visible)
            {
                if(listView.SelectedItem is SearchItem item)
                {
                    UpdateSelected(item);
                }
            }
        }
        DisplayItem[] search;
        void OnFocused()
        {
            listView.Visibility = Visibility.Collapsed;

            if (displaySource == null || !IsSearchEnabled)
                return;

            if (isPaste)
            {
                isPaste = false;
                return;
            }

            IEnumerable<SearchItem> res = default;

            string text = Text?.ToLower();


            if (!string.IsNullOrEmpty(text))
            {
                toggle.IsChecked = false;
                search = displaySource.Where(x => x.DisplayLower?.Contains(text) ?? false).ToArray();
                res = GetSearchItems(text, search);


                if (!search.Any(x => string.Equals(x.DisplayLower, text)))
                {
                    SelectedItem = null;
                    SelectedIndex = -1;
                }
                else
                {
                    SelectedItem = search.FirstOrDefault(x => string.Equals(x.DisplayLower, text)).Item;
                }
            }
            else
            {
                search = null;
                res = GetSearchItems(displaySource);
            }


            listView.ItemsSource = res;

            if (res.Count() > 0)
            {
                listView.Visibility = Visibility.Visible;
                listView.SelectedIndex = 0;
            }

        }

        private static IEnumerable<SearchItem> GetSearchItems(string text, DisplayItem[] search)
        {
            return search.Select(y =>
            {

                string nb1 = y.Display;
                string b = string.Empty;
                string nb2 = string.Empty;

                string x = y.DisplayLower;

                if (x.Equals(text))
                {
                    nb1 = string.Empty;
                    b = y.Display;
                }
                else if (x.IndexOf(text) > -1)
                {
                    nb1 = y.Display.Substring(0, x.IndexOf(text));
                    b = y.Display.Substring(x.IndexOf(text), text.Length);
                    nb2 = y.Display.Substring(x.IndexOf(text) + text.Length, x.Length - x.IndexOf(text) - text.Length);
                }

                var item = new SearchItem
                {
                    NotBoldPart1 = nb1,
                    BoldPart = b,
                    NotBoldPart2 = nb2,
                    DisplayProperty = y.Display,
                    Item = y.Item,
                };
                return item;
            });
        }
        private static IEnumerable<SearchItem> GetSearchItems(DisplayItem[] search)
        {
            return search.Select(y =>
            {
                var item = new SearchItem
                {
                    NotBoldPart1 = y.Display,
                    BoldPart = string.Empty,
                    NotBoldPart2 = string.Empty,
                    DisplayProperty = y.Display,
                    Item = y.Item,
                };
                return item;
            });
        }

        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (search != null && search.Length == 1 && search[0].DisplayLower == Text?.ToLower())
                return;

            OnFocused();
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            listView.Visibility = Visibility.Collapsed;
            beforeColl = null;
            toggle.IsChecked = false;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement f && f.DataContext is SearchItem item)
            {
                UpdateSelected(item);
            }
        }

        private void UpdateSelected(SearchItem item)
        {
            SelectedItem = item.Item;
            toggle.IsChecked = false;
            listView.Visibility = Visibility.Collapsed;
        }

        IEnumerable beforeColl;

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            listView.Visibility = Visibility.Visible;

            if (search != null && search.Length > 0)
            {
                var other = GetSearchItems(displaySource.Except(search, new DisplayItemComparer()).ToArray());
                beforeColl = listView.ItemsSource;
                listView.ItemsSource = listView.ItemsSource.OfType<SearchItem>().Union(other, new SearchItemComparer());
            }
            else if(displaySource != null)
            {
                listView.ItemsSource = GetSearchItems(displaySource);
            }
            else
            {
                beforeColl = null;
                listView.Visibility = Visibility.Collapsed;
            }

            tb.Focus();
            

        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if(beforeColl != null)
            {
                listView.ItemsSource = beforeColl;
                listView.Visibility = Visibility.Visible;
            }
            else
            {
                listView.Visibility = Visibility.Collapsed;

            }
            tb.Focus();
            //todo Показать только те элементы, где есть совпадения по тексту
        }

        #region OnChangedStatic
        private static void OnTextChagedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as TextBoxList;
            obj.OnTextChanged(sender, e);
        }

        private static void OnItemsSourceChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var obj = s as TextBoxList;
            obj.OnItemsSourceChanged(s, e);
        }

        private static void OnDisplayMemberChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var obj = s as TextBoxList;
            obj.OnDisplayMebmerPathChanged(s, e);
        }
        private static void OnSelectedIndexChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void OnSelectedItemChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var obj = s as TextBoxList;
            obj.OnSelectedItemChanged(s, e);
        }
        private static void OnSelectedValueChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var obj = s as TextBoxList;
            obj.OnSelectedValueChanged(s, e);
        }

        private static void OnSelectedValuePathChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var obj = s as TextBoxList;
            obj.OnSelectedValuePathChanged(s, e);
        }

        private static void OnIsSearchEnabledChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var obj = s as TextBoxList;
            obj.OnIsSearchEnabledChanged(s, e);
        }

        #endregion


    }

    class DisplayItemComparer : IEqualityComparer<DisplayItem>
    {
        public bool Equals([AllowNull] DisplayItem x, [AllowNull] DisplayItem y)
        {
            return x.Item.Equals(y.Item);
        }

        public int GetHashCode([DisallowNull] DisplayItem obj)
        {
            return obj.Item?.GetHashCode() ?? default;
        }
    }

    class SearchItemComparer : IEqualityComparer<SearchItem>
    {
        public bool Equals([AllowNull] SearchItem x, [AllowNull] SearchItem y)
        {
            return x?.Item == y?.Item;
        }

        public int GetHashCode([DisallowNull] SearchItem obj)
        {
            return obj?.Item?.GetHashCode() ?? default;
        }
    }
}
