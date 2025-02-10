using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace AutoCare.Components
{
    /// <summary>
    /// Interaction logic for Pagination.xaml
    /// </summary>
    public partial class Pagination : UserControl
    {


        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set
            {
                SetValue(CurrentPageProperty, value);
                UpdateButtons();
            }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(Pagination), new PropertyMetadata(1));



        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            set { SetValue(TotalPagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register("TotalPages", typeof(int), typeof(Pagination), new PropertyMetadata(1));


        private RadioButton[] radioButtons;

        public Pagination()
        {
            InitializeComponent();
            radioButtons = [btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9];
            Loaded += (e, s) => UpdateButtons();
        }

        private void Button_Next(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPages) CurrentPage++;
        }

        private void Button_Previous(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1) CurrentPage--;
        }

        private void Page_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Content is int val)
            {
                CurrentPage = val;
            }
        }

        private void UpdateButtons()
        {
            if (TotalPages <= 9)
            {
                lSeparator.Visibility = Visibility.Collapsed;
                rSeparator.Visibility = Visibility.Collapsed; 
                radioButtons[7].Visibility = Visibility.Visible;
                radioButtons[8].Visibility = Visibility.Visible;
                for (int i = 0; i < radioButtons.Length; i++)
                {
                    radioButtons[i].Content = i + 1;
                }
            }
            else
            {
                radioButtons[7].Visibility = Visibility.Collapsed;
                radioButtons[8].Visibility = Visibility.Collapsed;
                radioButtons[0].Content = 1;
                radioButtons[6].Content = TotalPages;
                if (CurrentPage < 7)
                {
                    lSeparator.Visibility = Visibility.Collapsed;
                    radioButtons[7].Visibility = Visibility.Visible;
                    radioButtons[7].Content = 2;
                    int start = 3;
                    for (int i = 1; i < 6; i++)
                    {
                        radioButtons[i].Content = start;
                        start++;
                    }
                }
                else if (CurrentPage > TotalPages - 6)
                {
                    rSeparator.Visibility = Visibility.Collapsed;
                    radioButtons[8].Visibility = Visibility.Visible;
                    radioButtons[8].Content = TotalPages - 1;
                    int start = TotalPages - 2;
                    for (int i = 5; i >= 1; i--)
                    {
                        radioButtons[i].Content = start;
                        start--;
                    }
                }
                else
                {
                    lSeparator.Visibility = Visibility.Visible;
                    rSeparator.Visibility = Visibility.Visible;
                    radioButtons[7].Visibility = Visibility.Collapsed;
                    radioButtons[8].Visibility = Visibility.Collapsed;

                    radioButtons[1].Content = CurrentPage - 2;
                    radioButtons[2].Content = CurrentPage - 1;
                    radioButtons[3].Content = CurrentPage;
                    radioButtons[4].Content = CurrentPage + 1;
                    radioButtons[5].Content = CurrentPage + 2;

                }
            }

            foreach (var btn in radioButtons)
            {
                if (int.TryParse(btn.Content.ToString(), out int page) && page == CurrentPage)
                {
                    btn.IsChecked = true;
                    return;
                }
            }
        }
    }
}
