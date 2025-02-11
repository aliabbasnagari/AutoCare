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
    public class DataPreloader
    {
        private static readonly Lazy<List<Item>> _dataCache = new Lazy<List<Item>>(LoadData);
        public static List<Item> Data => _dataCache.Value;
        private static List<Item> LoadData()
        {
            Debug.WriteLine("CALLED");
            return TestData.LoadItemsFromCsvResource("POS.csv");
        }
    }
    /// <summary>
    /// Interaction logic for InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Page
    {
        private List<Item> Items => DataPreloader.Data;
        private Paginator<Item> _paginator = new(DataPreloader.Data, 15);
        private CancellationTokenSource? _cancellationTokenSource;
        public ObservableCollection<Item> FilteredItems { get; } = new();
        public InventoryPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnPageLoaded;
            Unloaded += OnPageUnloaded;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            await ReloadItemsAsync();
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task ReloadItemsAsync()
        {
            _cancellationTokenSource?.Cancel();
            pagination.TotalPages = _paginator.TotalPages;
            //pagination.CurrentPage = _paginator.PageNumber();
            FilteredItems.Clear();
            try
            {
                var items = _paginator.GetCurrentPage();
                _cancellationTokenSource = new CancellationTokenSource();
                pbLoading.Visibility = Visibility.Visible;
                pbLoading.Value = 0;
                IProgress<double> progress = new Progress<double>(value => pbLoading.Value = value);

                var i = 0;
                var progressStep = items.Count / 100f;
                foreach (var item in items)
                {
                    i++;
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    FilteredItems.Add(item);
                    progress.Report(i / progressStep);
                    await Task.Yield();
                }
                progress.Report(100);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Item loading was cancelled.", "Cancelled",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                pbLoading.Visibility = Visibility.Collapsed;
            }
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

            // Apply the animation to the Width property of the Border (SlidingPanel)
            SlidingPanel.BeginAnimation(Border.WidthProperty, columnWidthAnimation);
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox)
            {
                var searchQuery = textbox.Text.ToLower();
                if (String.IsNullOrEmpty(searchQuery))
                {
                    // LoadFiltered(Items);
                    _paginator.UpdateItems(Items);
                    await ReloadItemsAsync();
                    return;
                }

                await SearchItemsAsync(searchQuery);
                return;
            }
            else
            {
                _paginator.UpdateItems(Items);
                await ReloadItemsAsync();
                return;
            }
        }

        private async Task SearchItemsAsync(string searchTerm)
        {
            pbLoading.Visibility = Visibility.Visible;
            var filteredItems = await Task.Run(() => ItemSearcher.SearchItems(Items.ToList(), searchTerm));
            _paginator.UpdateItems(filteredItems);
            await ReloadItemsAsync();
            pbLoading.Visibility = Visibility.Collapsed;
        }


        private void LoadFiltered(IEnumerable<Item> items)
        {
            FilteredItems.Clear();
            foreach (var item in items)
            {
                FilteredItems.Add(item);
            }
        }

        private void aus_AddUserClick(object sender, RoutedEventArgs e)
        {
            Items.Add(aus.GetItem());
            FilteredItems.Clear();
            foreach (var item in Items)
            {
                FilteredItems.Add(item);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var searchTerm = tbSearch.Text.Trim().ToLower();
            if (String.IsNullOrEmpty(searchTerm))
            {
                _paginator.UpdateItems(Items);
                await ReloadItemsAsync();
                return;
            }

            await SearchItemsAsync(searchTerm);
        }

        private async void pagination_PageChanged(object sender, int e)
        {
            Debug.WriteLine("pagination_PageChanged: " + e);
            _paginator.MoveToPage(e);
            await ReloadItemsAsync();
        }
    }
}
