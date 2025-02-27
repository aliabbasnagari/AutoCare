using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoCare.Components
{
    /// <summary>
    /// Interaction logic for ButtonNav.xaml
    /// </summary>
    public partial class ButtonNav : RadioButton
    {
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ButtonNav), new PropertyMetadata(new BitmapImage(new Uri("/Assets/Icons/Home.png", UriKind.Relative))));

        public ButtonNav()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
