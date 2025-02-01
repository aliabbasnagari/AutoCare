namespace AutoCare.Models
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Labels { get; set; }
        public string? Tags { get; set; }
        public double PurchasePrice { get; set; }
        public double RetailPrice { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public string? Location { get; set; }
        public double Markup => (RetailPrice - PurchasePrice) / PurchasePrice * 100;

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Description: {Description}, Labels: {Labels}, Tags: {Tags}, " +
                   $"BuyingPrice: {PurchasePrice:C}, SellingPrice: {RetailPrice:C}, Markup: {Markup}%, " +
                   $"Date: {UpdatedOn:yyyy-MM-dd}, StockQuantity: {StockQuantity}, SoldQuantity: {SoldQuantity}, Location: {Location}";

        }

        public string ConcatString()
        {
            return $"{Id} {Name} {Description} {Labels} {Tags} " +
                   $"{PurchasePrice} {RetailPrice} {Markup}% " +
                   $"{UpdatedOn:yyyy-MM-dd} {StockQuantity} {SoldQuantity} {Location}";
        }

        public string TextualString()
        {
            return $"{Name} {Description} {Labels} {Tags} {Location}";
        }
    }

}
