using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Services;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AutoCare.Views
{
    /// <summary>
    /// Interaction logic for InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Page
    {
        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<Item> FilteredItems { get; set; }
        public bool ShowSide { get; set; }

        public InventoryPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<Item>();
            FilteredItems = new ObservableCollection<Item>();
            this.DataContext = this;
            LoadDataAsync();

            string s1 = "Long search term";

            Debug.WriteLine(s1.Levenshtein("log search tm"));
            Debug.WriteLine(s1.LCS("log search tm"));


            Debug.WriteLine(s1.Levenshtein("Long search term"));
            Debug.WriteLine(s1.LCS("Long search term"));


            Debug.WriteLine(s1.Levenshtein("search term"));
            Debug.WriteLine(s1.LCS("search Term"));


        }

        private async void LoadDataAsync()
        {
            pbLoading.Visibility = Visibility.Visible;
            foreach (var item in TestData.LoadItemsFromCsv("C:\\Users\\Ali\\Downloads\\POS2.csv"))
            {
                await Task.Delay(5);
                Items.Add(item);
                FilteredItems.Add(item);
            }
            pbLoading.Visibility = Visibility.Collapsed;
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
