using AutoCare.MVVM;

namespace AutoCare.Models
{
    public class SaleItem : ViewModelBase
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public Item Item { get; set; }


        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value, nameof(Quantity), nameof(TotalPrice));
        }

        public double TotalPrice => Quantity * Item.RetailPrice;

        public SaleItem(Item item)
        {
            ItemId = item.Id;
            Item = item;
            Quantity = 1;
        }
    }
}
