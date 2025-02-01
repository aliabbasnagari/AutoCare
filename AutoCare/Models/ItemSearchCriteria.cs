namespace AutoCare.Models
{
    public class ItemSearchCriteria
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Labels { get; set; }
        public string? Tags { get; set; }
        public double? MinBuyingPrice { get; set; }
        public double? MaxBuyingPrice { get; set; }
        public double? MinSellingPrice { get; set; }
        public double? MaxSellingPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public int? MinStockQuantity { get; set; }
        public int? MinSoldQuantity { get; set; }
    }
}
