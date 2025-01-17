using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCare.Models
{
    public class SaleRecord
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public List<SaleItem> Items { get; set; }
        public DateTime Date { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal Change { get; set; }
    }
}
