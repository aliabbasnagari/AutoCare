using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AutoCare.Components
{
    /// <summary>
    /// Interaction logic for CategoryDeleteDialog.xaml
    /// </summary>
    public partial class CategoryDeleteDialog : Window
    {
        public bool DeleteSubcategories { get; private set; } = true;
        public CategoryDeleteDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            DeleteSubcategories = rbDelete.IsChecked ?? true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }



    }
}
