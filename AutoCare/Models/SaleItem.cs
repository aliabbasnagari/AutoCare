using AutoCare.MVVM;

namespace AutoCare.Models
{
    public class SaleItem : ViewModelBase
    {

        public int Id { get; set; }
        public int ItemId { get; set; }
        public double Price { get; set; }

        private int quantity;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public double TotalPrice => Quantity * Price;
    }
}
