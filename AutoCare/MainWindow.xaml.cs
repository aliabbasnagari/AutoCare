using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace AutoCare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var vCulture = new CultureInfo("en-PK");
            Thread.CurrentThread.CurrentCulture = vCulture;
            Thread.CurrentThread.CurrentUICulture = vCulture;
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
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