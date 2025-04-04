using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoCare.Components;
using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Services;
using AutoCare.Views;
using Lucene.Net.Index;

namespace JustInventory
{
    public partial class DataPreloader
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<Item> Items => DataPreloader.Data;
        //public ObservableCollection<Item> FilteredItems { get; } = new();
        private Paginator<Item> _paginator = new(DataPreloader.Data, 15);

        private ObservableCollection<Item> _currentPageItems = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ObservableCollection<Item> CurrentPageItems
        {
            get { return _currentPageItems; }
            set { _currentPageItems = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnPageLoaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            ReloadItems();
        }

        private void ReloadItems()
        {

            pbLoading.Visibility = Visibility.Visible;
            //CurrentPageItems.Clear();

            var items = _paginator.GetCurrentPage();
            Debug.WriteLine(items.Count);
            CurrentPageItems = new ObservableCollection<Item>(items);

            pbLoading.Visibility = Visibility.Collapsed;


            //try
            //{
            //    var items = _paginator.GetCurrentPage();
            //    _cancellationTokenSource = new CancellationTokenSource();
            //    pbLoading.Visibility = Visibility.Visible;
            //    foreach (var item in items)
            //    {
            //        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            //        CurrentPageItems.Add(item);
            //    }
            //}
            //catch (OperationCanceledException)
            //{
            //    MessageBox.Show("Item loading was cancelled.", "Cancelled",
            //        MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error loading items: {ex.Message}", "Error",
            //        MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            //finally
            //{
            //    pbLoading.Visibility = Visibility.Collapsed;
            //}
        }



        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox)
            {
                var searchQuery = textbox.Text.ToLower();
                if (String.IsNullOrEmpty(searchQuery))
                {
                    _paginator.UpdateItems(Items);
                    ReloadItems();
                    return;
                }

                await SearchItemsAsync(searchQuery);
                return;
            }
            else
            {
                _paginator.UpdateItems(Items);
                ReloadItems();
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


        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var searchTerm = tbSearch.Text.Trim().ToLower();
            if (String.IsNullOrEmpty(searchTerm))
            {
                _paginator.UpdateItems(Items);
                ReloadItems();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                Debug.WriteLine(btn.Content);
                if (btn.Content?.Equals("Prev") == true)
                {
                    _paginator.PreviousPage();
                    ReloadItems();
                } else if (btn.Content?.Equals("Next") == true)
                {
                    _paginator.NextPage();
                    ReloadItems();
                }

            }
        }
    }
}