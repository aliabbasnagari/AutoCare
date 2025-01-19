using AutoCare.Views;
using System.Windows;

namespace AutoCare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            rootFrame.Navigate(new ManageCategoriesPage());
            //rootFrame.Navigate(new InventoryPage());
        }
    }
}