namespace AutoCare.Models
{
    public class SaleRecord
    {
        public int Id { get; set; }

        public string Customer { get; set; } = "N/A";

        public List<SaleItem> Items { get; set; } = new List<SaleItem>();

        public DateTime Date { get; set; }

        public double SubTotal => Items.Sum(item => item.TotalPrice);

        public double Tax { get; set; }

        public double Discount { get; set; }

        public double Total => SubTotal + Tax - Discount;

        public string? Status { get; set; }

        public double ReceivedAmount { get; set; }

        public double ReturnChange => ReceivedAmount - Total;
    }

}
