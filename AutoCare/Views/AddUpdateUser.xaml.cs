using AutoCare.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

namespace AutoCare.Views
{

    /// <summary>
    /// Interaction logic for AddUpdateUser.xaml
    /// </summary>
    public partial class AddUpdateUser : UserControl
    {
        private double _buyingPrice;
        private double _sellingPrice;
        private double _markup;

        public int Id { get; set; }
        public string Name => NameTextBox.Text;
        public string Description => DescriptionTextBox.Text;
        public string Labels => LabelsTextBox.Text;
        public string Tags => TagsTextBox.Text;
        public double BuyingPrice => Double.TryParse(BuyingPriceTextBox.Text, out var val) ? val : 0;
        public double SellingPrice => Double.TryParse(SellingPriceTextBox.Text, out var val) ? val : 0;
        public double Markup => Double.TryParse(MarkupText.Text, out var val) ? val : 0;
        public int StockQuantity => int.TryParse(StockQuantityTextBox.Text, out var result) ? result : 0;
        public int SoldQuantity => int.TryParse(SoldQuantityTextBox.Text, out var result) ? result : 0;
        public string Location => LocationTextBox.Text;
        public DateTime Date => DatePicker.SelectedDate.GetValueOrDefault();

        public static readonly RoutedEvent AddUserClickEvent = EventManager.RegisterRoutedEvent(
            "AddUserClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(AddUpdateUser));

        // Provide CLR accessors for the custom Click event
        public event RoutedEventHandler AddUserClick
        {
            add { AddHandler(AddUserClickEvent, value); }
            remove { RemoveHandler(AddUserClickEvent, value); }
        }


        public AddUpdateUser()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Now;
        }

        public Item GetItem()
        {
            return new Item
            {
                Id = Id,
                Name = Name,
                Description = Description,
                PurchasePrice = BuyingPrice,
                RetailPrice = SellingPrice,
                Markup = Markup,
                StockQuantity = StockQuantity,
                SoldQuantity = SoldQuantity,
                Location = Location,
                Tags = Tags,
                Labels = Labels,
                UpdatedOn = Date
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(AddUserClickEvent, sender));
        }

        // Buying Price TextBox TextChanged handler
        private void BuyingSellingPriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(BuyingPriceTextBox.Text, out decimal buyingPrice) &&
                decimal.TryParse(SellingPriceTextBox.Text, out decimal markup))
            {
                MarkupText.Text = $"{(SellingPrice - BuyingPrice) / BuyingPrice * 100:F3}";
            }
        }
    }
}
