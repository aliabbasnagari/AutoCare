using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Services;
using Lucene.Net.Index;

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

        public List<Item> Items => DataPreloader.Data;
        public List<Item> FilteredItems { get; } = new();
        public ObservableCollection<Item> ItemsToDisplay { get; } = new();


        private CancellationTokenSource? _cancellationTokenSource;

        private Paginator<Item> _paginator;

        public InventoryPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnPageLoaded;
            Unloaded += OnPageUnloaded;
            _paginator = new Paginator<Item>(Items.ToList(), 20);
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            await LoadItemsAsync();
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task LoadItemsAsync()
        {
            FilteredItems.Clear();
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                pbLoading.Visibility = Visibility.Visible;
                pbLoading.Value = 0;

                //var items = await Task.Run(() =>
                //    TestData.LoadItemsFromCsvResource("POS.csv"),
                //    _cancellationTokenSource.Token);

                IProgress<double> progress = new Progress<double>(value =>
                    pbLoading.Value = value);

                var items = _paginator.CurrentPage();

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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox)
            {
                var searchQuery = textbox.Text.ToLower();
                if (String.IsNullOrEmpty(searchQuery))
                {
                    LoadFiltered(Items);
                    return;
                }

                SearchItems(searchQuery);
                return;


                FilteredItems.Clear();
                //var search = Items.Where(i => SearchMatch(searchText, i) > 0);
                var search = Items.Select(i => new { Item = i, Weight = SearchMatch(searchQuery, i) })
                    .Where(x => x.Weight > 0)
                    .OrderByDescending(x => x.Weight)
                    .Select(x => x.Item);

                foreach (var item in search)
                {
                    FilteredItems.Add(item);
                }
            }
            else
            {
                LoadFiltered(Items);
                return;
            }
        }

        private void SearchItems(string searchTerm)
        {
            LoadFiltered(ItemSearcher.SearchItems(Items.ToList(), searchTerm));
        }

        private async void SearchItemsAsync(string searchTerm)
        {
            pbLoading.Visibility = Visibility.Visible;

            await Task.Delay(1000);
            // Run the search operation in a background task
            var filteredItems = await Task.Run(() => ItemSearcher.SearchItems(Items.ToList(), searchTerm));

            // Update the UI on the main thread once the search is complete
            LoadFiltered(filteredItems);

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

        private double SearchMatch(string search, Item item)
        {
            double matchWeight = 0;
            string str2 = item.ConcatString().ToLower();
            for (int length = 1; length <= search.Length; length++)
            {
                for (int start = 0; start <= search.Length - length; start++)
                {
                    string substring = search.Substring(start, length).ToLower();
                    if (str2.Contains(substring))
                    {
                        matchWeight += substring.Length;
                    }
                }
            }
            return matchWeight;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var searchTerm = tbSearch.Text.Trim().ToLower();
            if (String.IsNullOrEmpty(searchTerm))
            {
                LoadFiltered(Items);
                return;
            }

            SearchItemsAsync(searchTerm);
        }
    }
}
