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
using System.Windows.Shapes;

namespace memo
{
    /// <summary>
    /// Interaction logic for NewMemo.xaml
    /// </summary>
    public partial class NewMemo : Window
    {
        public NewMemo()
        {
            InitializeComponent();

            txtName.Focus();
        }

        public string MemoTitle
        {
            get;
            set;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            MemoTitle = txtName.Text;
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
