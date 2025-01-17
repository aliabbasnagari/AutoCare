using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCare.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Labels { get; set; }
        public string? Tags { get; set; }
        public double BuyingPrice { get; set; }
        public double SellingPrice { get; set; }
        public double Markup { get; set; }
        public DateTime Date { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public string Location { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Description: {Description}, Labels: {Labels}, Tags: {Tags}, " +
                   $"BuyingPrice: {BuyingPrice:C}, SellingPrice: {SellingPrice:C}, Markup: {Markup}%, " +
                   $"Date: {Date:yyyy-MM-dd}, StockQuantity: {StockQuantity}, SoldQuantity: {SoldQuantity}, Location: {Location}";

        }

        public string ConcatString()
        {
            return $"{Id} {Name} {Description} {Labels} {Tags} " +
                   $"{BuyingPrice} {SellingPrice} {Markup}% " +
                   $"{Date:yyyy-MM-dd} {StockQuantity} {SoldQuantity} {Location}";
        }
    }

}
