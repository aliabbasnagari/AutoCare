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
        private List<Item> Items => DataPreloader.Data;
        private Paginator<Item> _paginator = new Paginator<Item>(DataPreloader.Data, 10);
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
            CreatePagination(_paginator);
            await ReloadItemsAsync();
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task ReloadItemsAsync()
        {
            FilteredItems.Clear();
            try
            {
                var items = _paginator.CurrentPage();
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

        private void CreatePagination(Paginator<Item> paginator)
        {
            dynamicPagination.Children.Clear();
            if (paginator.TotalPages - paginator.PageNumber() < 7)
            {
                for (int i = paginator.PageNumber(); i < paginator.TotalPages; i++)
                {
                    Button btn = new Button
                    {
                        Content = $"{i}",
                        Tag = i,
                        Margin = new Thickness(5)
                    }; 
                    btn.Click += (s, e) => Page_Navigation(s, e);
                    dynamicPagination.Children.Add(btn);
                }
            }
            else
            {
                for (int i = _paginator.PageNumber(); i < _paginator.PageNumber() + 3; i++)
                {
                    Button btn = new Button
                    {
                        Content = $"{i + 1}",
                        Tag = i,
                        Margin = new Thickness(5),
                    };
                    btn.Click += (s, e) => Page_Navigation(s, e);
                    dynamicPagination.Children.Add(btn);
                }

                dynamicPagination.Children.Add(new TextBlock
                {
                    Text = "..."
                });


                for (int i = _paginator.TotalPages - 3; i < _paginator.TotalPages; i++)
                {
                    Button btn = new Button
                    {
                        Content = $"{i}",
                        Tag = i,
                        Margin = new Thickness(5)
                    };
                    btn.Click += (s, e) => Page_Navigation(s, e);
                    dynamicPagination.Children.Add(btn);
                }
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
                    _paginator = new Paginator<Item>(Items, 10);
                    await ReloadItemsAsync();
                    return;
                }

                await SearchItemsAsync(searchQuery);
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
                // LoadFiltered(Items);
                _paginator = new Paginator<Item>(Items, 10);
                await ReloadItemsAsync();
                return;
            }
        }

        //private async Task SearchItems(string searchTerm)
        //{
        //    // LoadFiltered(ItemSearcher.SearchItems(Items.ToList(), searchTerm));
        //    await LoadItemsAsync(ItemSearcher.SearchItems(Items.ToList(), searchTerm));
        //}

        private async Task SearchItemsAsync(string searchTerm)
        {
            pbLoading.Visibility = Visibility.Visible;

            await Task.Delay(1000);
            // Run the search operation in a background task
            var filteredItems = await Task.Run(() => ItemSearcher.SearchItems(Items.ToList(), searchTerm));
            _paginator = new Paginator<Item>(filteredItems, 10);
            // Update the UI on the main thread once the search is complete
            // LoadFiltered(filteredItems);
            await ReloadItemsAsync();

            pbLoading.Visibility = Visibility.Collapsed;
        }


        //private void LoadFiltered(IEnumerable<Item> items)
        //{
        //    FilteredItems.Clear();
        //    foreach (var item in items)
        //    {
        //        FilteredItems.Add(item);
        //    }
        //}

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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var searchTerm = tbSearch.Text.Trim().ToLower();
            if (String.IsNullOrEmpty(searchTerm))
            {
                // LoadFiltered(Items);
                _paginator = new Paginator<Item>(Items, 10);
                await ReloadItemsAsync();
                return;
            }

            await SearchItemsAsync(searchTerm);
        }

        private async void Page_Navigation(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {

                if (int.TryParse(btn.Tag?.ToString(), out int tag))
                {
                    _paginator.MoveToPage(tag);
                    await ReloadItemsAsync();
                }


                switch (btn.Tag)
                {
                    case "Next":
                        _paginator.NextPage();
                        await ReloadItemsAsync();
                        break;
                    case "Previous":
                        _paginator.PreviousPage();
                        await ReloadItemsAsync();
                        break;
                }
                CreatePagination(_paginator);
            }

        }
    }
}
