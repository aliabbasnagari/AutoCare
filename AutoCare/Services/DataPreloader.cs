using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCare.Data;
using AutoCare.Models;

namespace AutoCare.Services
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
}
