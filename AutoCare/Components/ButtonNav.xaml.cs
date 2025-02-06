using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            Checked += OnCheckedChanged;
            Unchecked += OnCheckedChanged;
            Loaded += OnCheckedChanged;
        }

        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            Line? myLine = this.Template.FindName("myLine", this) as Line;
            if (myLine != null)
            {
                myLine.Visibility = Visibility.Visible;
                DoubleAnimation da = new DoubleAnimation
                {
                    From = (IsChecked == true) ? 0 : 65,
                    To = (IsChecked == true) ? 65 : 0,
                    Duration = TimeSpan.FromMilliseconds(150),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                if (IsChecked != true)
                    da.Completed += (s, e) => myLine.Visibility = Visibility.Collapsed;

                myLine.BeginAnimation(Line.Y2Property, da);
            }
        }
    }
}
