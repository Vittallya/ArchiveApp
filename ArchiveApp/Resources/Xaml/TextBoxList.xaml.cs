using System;
using System.Collections;
using System.Collections.Generic;
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
        #endregion

        private void OnTextChaged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null)
            {
                Type srcType = ItemsSource.GetType();
                itemType = srcType.GetElementType();

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
                NewMethod1();
            }
            else
            {
                NewMethod2();
            }
        }

        private void NewMethod2()
        {
            displaySource = ItemsSource.Cast<object>().Select(x =>
            {
                return new DisplayItem(x.ToString(), x);

            }). ToArray();
        }

        private void NewMethod1()
        {
            SetupProperty(itemType, DisplayMemberPath, ref displayProperty);
            displaySource = ItemsSource.Cast<object>()?.Select(x =>
            {
                string display = displayProperty?.GetValue(x)?.ToString();
                return new DisplayItem(display, x);

            })?.ToArray();
        }

        private void OnDisplayMebmerPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (itemType != null)
            {
                SetupProperty(itemType, DisplayMemberPath, ref displayProperty);
                SetupDisplayPath();
            }
        }

        private void OnSelectedValuePathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (itemType != null && e.NewValue != null)
            {
                SetupProperty(itemType, SelectedValuePath, ref valueProperty);
            }
        }
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

        void OnFocused()
        {
            if (displaySource == null)
                return;

            if (isPaste)
            {
                isPaste = false;
                return;
            }

            string text = tb.Text.ToLower();

            var search = displaySource.Where(x => x.Display?.ToLower()?.Contains(text) ?? false);

            var res = search.Select(y =>
            {
                string x = y.Display.ToLower();

                string nb1 = y.Display.Substring(0, x.IndexOf(text));
                string b = y.Display.Substring(x.IndexOf(text), text.Length);
                string nb2 = y.Display.Substring(x.IndexOf(text) + text.Length, x.Length - x.IndexOf(text) - text.Length);

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
            listView.ItemsSource = res;

            if (res.Count() > 0)
            {
                listView.Visibility = Visibility.Visible;
                listView.SelectedIndex = 0;
            }
            
        }

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnFocused();
        }

        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            OnFocused();
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            listView.Visibility = Visibility.Collapsed;
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
            isPaste = true;
            tb.Text = item.DisplayProperty;
            listView.Visibility = Visibility.Collapsed;
            SelectedItem = item.Item;

            if (valueProperty != null)
            {
                SelectedValue = valueProperty.GetValue(item.Item);
            }
        }

        private static void OnTextChagedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as TextBoxList;
            obj.OnTextChaged(sender, e);
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

        }

        private static void OnSelectedValueChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnSelectedValuePathChangedStatic(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var obj = s as TextBoxList;
            obj.OnSelectedValuePathChanged(s, e);

        }

    }
}
