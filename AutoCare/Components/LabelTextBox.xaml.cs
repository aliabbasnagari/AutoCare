using System.Windows;
using System.Windows.Controls;

namespace AutoCare.Components
{
    /// <summary>
    /// Interaction logic for LabelTextBox.xaml
    /// </summary>
    public partial class LabelTextBox : UserControl
    {

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(LabelTextBox), new PropertyMetadata(String.Empty));





        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LabelTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));






        public LabelTextBox()
        {
            InitializeComponent();
        }
    }
}
