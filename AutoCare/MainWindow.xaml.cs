using AutoCare.Views;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
            rootFrame.Navigate(new Uri($"Views/SalesPage.xaml", UriKind.Relative));
        }

        private void NavigateToPage(object sender, RoutedEventArgs e)
        {
            if (sender is ButtonBase button && button.Tag != null)
            {
                rootFrame.Navigate(new Uri($"Views/{button.Tag}.xaml", UriKind.Relative));
            }
        }
    }
}