using System.Windows;

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
