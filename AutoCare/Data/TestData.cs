using AutoCare.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoCare.Data
{
    public class TestData
    {
     
        public static List<Item> LoadItemsFromCsvResource(string resource)
        {
            string resourcePath = $"AutoCare;component/Assets/{resource}";
            var uri = new System.Uri(resourcePath, System.UriKind.Relative);
            var streamInfo = Application.GetResourceStream(uri);
            using var reader = new StreamReader(streamInfo.Stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return new List<Item>(csv.GetRecords<Item>());
        }

        public static List<Item> LoadItemsFromCsv(string filePath)
        {
            var items = new List<Item>();
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            items = new List<Item>(csv.GetRecords<Item>());
            return items;
        }

        public static List<Item> GetItems2()
        {
            var items = new List<Item>
{
    new Item { Id = 1, Name = "Laptop", Description = "High-performance laptop", Labels = "Electronics", Tags = "Portable,Work", PurchasePrice = 800.00, RetailPrice = 1000.00, UpdatedOn = DateTime.Now, StockQuantity = 50, SoldQuantity = 10, Location = "A1" },
    new Item { Id = 2, Name = "Smartphone", Description = "Latest model smartphone", Labels = "Electronics", Tags = "Mobile,Smart", PurchasePrice = 500.00, RetailPrice = 650.00, UpdatedOn = DateTime.Now, StockQuantity = 100, SoldQuantity = 25, Location = "B2" },
    new Item { Id = 3, Name = "Headphones", Description = "Noise-cancelling headphones", Labels = "Accessories", Tags = "Audio,Music", PurchasePrice = 150.00, RetailPrice = 220.00, UpdatedOn = DateTime.Now, StockQuantity = 75, SoldQuantity = 30, Location = "C3" },
    new Item { Id = 4, Name = "Office Chair", Description = "Ergonomic office chair", Labels = "Furniture", Tags = "Comfort,Work", PurchasePrice = 120.00, RetailPrice = 180.00, UpdatedOn = DateTime.Now, StockQuantity = 40, SoldQuantity = 15, Location = "D4" },
    new Item { Id = 5, Name = "Desk Lamp", Description = "Adjustable desk lamp", Labels = "Lighting", Tags = "LED,Study", PurchasePrice = 30.00, RetailPrice = 45.00, UpdatedOn = DateTime.Now, StockQuantity = 60, SoldQuantity = 20, Location = "E5" },
    new Item { Id = 6, Name = "Backpack", Description = "Waterproof travel backpack", Labels = "Bags", Tags = "Travel,Outdoor", PurchasePrice = 40.00, RetailPrice = 65.00, UpdatedOn = DateTime.Now, StockQuantity = 90, SoldQuantity = 35, Location = "F6" },
    new Item { Id = 7, Name = "Tablet", Description = "10-inch Android tablet", Labels = "Electronics", Tags = "Portable,Media", PurchasePrice = 200.00, RetailPrice = 280.00,  UpdatedOn = DateTime.Now, StockQuantity = 70, SoldQuantity = 18, Location = "G7" },
    new Item { Id = 8, Name = "Wireless Mouse", Description = "Rechargeable wireless mouse", Labels = "Accessories", Tags = "Computer,Gadget", PurchasePrice = 20.00, RetailPrice = 35.00,  UpdatedOn = DateTime.Now, StockQuantity = 120, SoldQuantity = 50, Location = "H8" },
    new Item { Id = 9, Name = "Water Bottle", Description = "Insulated stainless steel bottle", Labels = "Kitchenware", Tags = "Hydration,Outdoor", PurchasePrice = 15.00, RetailPrice = 25.00,  UpdatedOn = DateTime.Now, StockQuantity = 150, SoldQuantity = 60, Location = "I9" },
    new Item { Id = 10, Name = "Gaming Keyboard", Description = "RGB mechanical keyboard", Labels = "Electronics", Tags = "Gaming,PC", PurchasePrice = 70.00, RetailPrice = 110.00,  UpdatedOn = DateTime.Now, StockQuantity = 45, SoldQuantity = 12, Location = "J10" }
};
            return items;
        }
        public static List<Item> GetItems()
        {
            List<Item> items = new List<Item>
            {
                new Item
                {
                    Id = 1,
                    Name = "Item 1",
                    Description = "Description of Item 1",
                    Labels = "Label1, Label2",
                    Tags = "Tag1, Tag2",
                    PurchasePrice = 100,
                    RetailPrice = 150,
                    UpdatedOn = DateTime.Now,
                    StockQuantity = 100,
                    SoldQuantity = 50,
                    Location = "Location 1"
                },
                new Item
                {
                    Id = 2,
                    Name = "Item 2",
                    Description = "Description of Item 2",
                    Labels = "Label3, Label4",
                    Tags = "Tag3, Tag4",
                    PurchasePrice = 120,
                    RetailPrice = 180,
                    UpdatedOn = DateTime.Now,
                    StockQuantity = 200,
                    SoldQuantity = 30,
                    Location = "Location 2"
                },
                new Item
                {
                    Id = 3,
                    Name = "Item 3",
                    Description = "Description of Item 3",
                    Labels = "Label5, Label6",
                    Tags = "Tag5, Tag6",
                    PurchasePrice = 90,
                    RetailPrice = 135,
                    UpdatedOn = DateTime.Now,
                    StockQuantity = 150,
                    SoldQuantity = 70,
                    Location = "Location 3"
                },
                 new Item
                {
                    Id = 4,
                    Name = "Item 4",
                    Description = "Description of Item 4, A long test description. This is a test long description. Hello is this large or long description.",
                    Labels = "Label5, Label6",
                    Tags = "Tag5, Tag6",
                    PurchasePrice = 90,
                    RetailPrice = 135,
                    UpdatedOn = DateTime.Now,
                    StockQuantity = 150,
                    SoldQuantity = 70,
                    Location = "Location 3"
                }
            };
            return items;

        }
    }
}
