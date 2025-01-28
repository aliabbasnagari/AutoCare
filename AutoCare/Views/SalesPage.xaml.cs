using AutoCare.Data;
using AutoCare.Models;
using AutoCare.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoCare.Views
{
    /// <summary>
    /// Interaction logic for SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        private SaleRecord Record { get; }
        public SalesPage()
        {
            InitializeComponent();
            Record = new SaleRecord();
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
                        //sItem.Quantity++;
                        Record.Items.Remove(sItem);
                        Record.Items.Add(new SaleItem
                        {
                            Id = Record.Items.Count + 1,
                            ItemId = sItem.ItemId,
                            Price = sItem.Price,
                            Quantity = sItem.Quantity + 1
                        });
                        sItem = null;
                    }
                    else
                    {
                        Record.Items.Add(new SaleItem
                        {
                            Id = Record.Items.Count + 1,
                            ItemId = item.Id,
                            Price = item.RetailPrice,
                            Quantity = 1
                        });
                    }

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
                    Record.Items.Add(new SaleItem
                    {
                        Id = Record.Items.Count + 1,
                        ItemId = item.Id,
                        Price = item.RetailPrice,
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
