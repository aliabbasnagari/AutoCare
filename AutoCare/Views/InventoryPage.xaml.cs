using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Services;

namespace AutoCare.Views
{
    /// <summary>
    /// Interaction logic for InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Page
    {
        private List<Item> Items => DataPreloader.Data;
        private Paginator<Item> _paginator = new(DataPreloader.Data, 30);
        private CancellationTokenSource? _cancellationTokenSource;
        // public ObservableCollection<Item> FilteredItems { get; } = new();
        public ObservableCollection<Item> CurrentPageItems
        {
            get { return (ObservableCollection<Item>)GetValue(CurrentPageItemsProperty); }
            set { SetValue(CurrentPageItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPageItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageItemsProperty = DependencyProperty.Register("CurrentPageItems",
            typeof(ObservableCollection<Item>),
            typeof(InventoryPage),
            new PropertyMetadata(new ObservableCollection<Item>()));



        public InventoryPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnPageLoaded;
            //Unloaded += OnPageUnloaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            pbLoading.Visibility = Visibility.Visible;
            ReloadItems();
            pbLoading.Visibility = Visibility.Collapsed;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private void ReloadItems()
        {
            _cancellationTokenSource?.Cancel();
            pagination.TotalPages = _paginator.TotalPages;
            pagination.CurrentPage = _paginator.PageNumber();
            var items = _paginator.GetCurrentPage();
            CurrentPageItems = new ObservableCollection<Item>(items);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation columnWidthAnimation = new DoubleAnimation
            {
                From = SlidingPanel.Width,
                To = 340 - SlidingPanel.Width,
                Duration = new Duration(System.TimeSpan.FromSeconds(0.5)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
            };
            SlidingPanel.BeginAnimation(Border.WidthProperty, columnWidthAnimation);
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox)
            {
                var searchQuery = textbox.Text.ToLower();
                if (String.IsNullOrEmpty(searchQuery))
                {
                    pbLoading.Visibility = Visibility.Visible;
                    _paginator.UpdateItems(Items);
                    ReloadItems();
                    pbLoading.Visibility = Visibility.Collapsed;
                    return;
                }

                await SearchItemsAsync(searchQuery);
                return;
            }
            else
            {
                pbLoading.Visibility = Visibility.Visible;
                _paginator.UpdateItems(Items);
                ReloadItems();
                pbLoading.Visibility = Visibility.Collapsed;
                return;
            }
        }

        private async Task SearchItemsAsync(string searchTerm)
        {
            pbLoading.Visibility = Visibility.Visible;
            var filteredItems = await Task.Run(() => ItemSearcher.SearchItems(Items.ToList(), searchTerm));
            _paginator.UpdateItems(filteredItems);
            ReloadItems();
            pbLoading.Visibility = Visibility.Collapsed;
        }

        private void aus_AddUserClick(object sender, RoutedEventArgs e)
        {
            Items.Add(aus.GetItem());
            //FilteredItems.Clear();
            foreach (var item in Items)
            {
                // FilteredItems.Add(item);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var searchTerm = tbSearch.Text.Trim().ToLower();
            if (String.IsNullOrEmpty(searchTerm))
            {
                pbLoading.Visibility = Visibility.Visible;
                _paginator.UpdateItems(Items);
                ReloadItems();
                pbLoading.Visibility = Visibility.Collapsed;
                return;
            }

            await SearchItemsAsync(searchTerm);
        }

        private void pagination_PageChanged(object sender, int e)
        {
            Debug.WriteLine("pagination_PageChanged: " + e);
            _paginator.MoveToPage(e);
            ReloadItems();
        }
    }
}
