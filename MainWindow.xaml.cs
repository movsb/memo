using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Collections.ObjectModel;

namespace memo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MemoManager _memo_manager;
        private ObservableCollection<MemoObject> _memo_objects;

        public MainWindow()
        {
            InitializeComponent();

            _memo_manager = new MemoManager();
            _memo_objects = new ObservableCollection<MemoObject>(_memo_manager.GetAll());
            lstMemos.ItemsSource = _memo_objects;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as FrameworkElement).Tag.ToString()) {
            case "new": {
                var wnd = new NewMemo()
                {
                    Owner = this
                };
                var ret = wnd.ShowDialog();
                if (ret.HasValue && ret.Value) {
                    var m = new MemoObject()
                    {
                        Title = wnd.MemoTitle,
                        Content = "",
                    };

                    _memo_objects.Insert(0, m);
                    m.Id = _memo_manager.Add(m);

                    lstMemos.SelectedItem = m;
                }
                break;
            }
            case "modify": {
                var m = lstMemos.SelectedItem as MemoObject;
                if (m == null) {
                    return;
                }
                var wnd = new NewMemo(m.Title)
                {
                    Owner = this
                };
                var ret = wnd.ShowDialog();
                if (ret.HasValue && ret.Value) {
                    m.Title = wnd.MemoTitle;
                }
                break;
            }
            case "delete": {
                var m = lstMemos.SelectedItem as MemoObject;
                if (m == null) {
                    return;
                }

                if (MessageBox.Show(this, "确定要删除此条备忘？", "Memo",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.OK)
                {
                    _memo_objects.Remove(m);
                    _memo_manager.Delete(m.Id);
                    if (grdContentWrapper.DataContext == m) {
                        grdContentWrapper.DataContext = null;
                    }
                }
                break;
            }
            }
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
        }

        private void lstMemos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) {
                var m = e.AddedItems[0] as MemoObject;
                grdContentWrapper.DataContext = m;
            } else {
                grdContentWrapper.DataContext = null;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            foreach (MemoObject M in _memo_objects) {
                if (M.Modified) {
                    _memo_manager.Update(M);
                    M.Modified = false;
                }
            }
        }
    }

    public class Null2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Bool2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
