using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoCare.Data;
using AutoCare.Models;
using AutoCare.MVVM;
using AutoCare.Services;

namespace AutoCare.Views
{
    /// <summary>
    /// Interaction logic for SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        private SaleRecordViewModel Record { get; set; }
        public SalesPage()
        {
            InitializeComponent();
            Record = new SaleRecordViewModel();
            DataContext = Record;
        }

        private void tbSearch_TextChanged(object sender, RoutedEventArgs e)
        {
            var searchQuery = tbSearch.Text.Trim().ToLower();
            if (String.IsNullOrEmpty(searchQuery))
            {
                lbSearch.ItemsSource = null;
                lbSearch.Visibility = System.Windows.Visibility.Collapsed;

                return;
            }

            SearchItemsAsync(searchQuery);
        }

        private async void SearchItemsAsync(string searchTerm)
        {
            // var items = TestData.LoadItemsFromCsv("C:\\Users\\Ali\\Downloads\\POS2.csv");
            var items = TestData.LoadItemsFromCsvResource("POS.csv");
            var filteredItems = await Task.Run(() => ItemSearcher.SearchItems(items, searchTerm));
            lbSearch.ItemsSource = filteredItems;
            lbSearch.Visibility = Visibility.Visible;
            //lbSearch.SelectedIndex = 0;
            //lbSearch.Focus();
        }


        private void lbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // If an item is selected, collapse the ListView
                if (lbSearch.SelectedItem != null)
                {
                    // Perform your desired action with the selected item (e.g., message box)
                    // MessageBox.Show($"Selected: {lbSearch.SelectedItem}");
                    var item = (Item)lbSearch.SelectedItem;
                    var sItem = Record.Items.FirstOrDefault(s => s.ItemId == item.Id);
                    if (sItem != null)
                    {
                        sItem.Quantity++;
                        //Record.Items.Remove(sItem);
                        //Record.Items.Add(new SaleItem(item)
                        //{
                        //    Id = Record.Items.Count + 1,
                        //    ItemId = sItem.ItemId,
                        //    Quantity = sItem.Quantity + 1
                        //});
                        //sItem = null;
                    }
                    else
                    {
                        Record.Items.Add(new SaleItem(item)
                        {
                            Id = Record.Items.Count + 1,
                            Quantity = 1
                        });
                    }

                    item = null;
                    sItem = null;

                    // Collapse the ListView
                    lbSearch.ItemsSource = null;
                    lbSearch.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void lbSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // If an item is selected, collapse the ListView
            if (lbSearch.SelectedItem != null)
            {
                // Perform your desired action with the selected item (e.g., message box)
                var item = (Item)lbSearch.SelectedItem;
                var sItem = Record.Items.FirstOrDefault(s => s.ItemId == item.Id);
                if (sItem != null)
                {
                    sItem.Quantity++;
                }
                else
                {
                    Record.Items.Add(new SaleItem(item)
                    {
                        Id = Record.Items.Count + 1,
                        Quantity = 1
                    });
                }

                // Collapse the ListView
                lbSearch.ItemsSource = null;
                lbSearch.Visibility = Visibility.Collapsed;
            }

        }
    }
}
