using AutoCare.Models;
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

namespace AutoCare.Views
{
    /// <summary>
    /// Interaction logic for ManageCategoriesPage.xaml
    /// </summary>
    public partial class ManageCategoriesPage : Page
    {
        private CategoryManager CategoryManager { get; set; }
        public Category? SelectedCategory { get; set; }
        public ManageCategoriesPage()
        {
            InitializeComponent();
            CategoryManager = new CategoryManager();

            tvCategories.DataContext = CategoryManager;
            tvCategories.ItemsSource = CategoryManager.Categories;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
            {
                CategoryManager.AddSubCategory(button.Tag.ToString(), "New Category");
            }
        }
    }
}
