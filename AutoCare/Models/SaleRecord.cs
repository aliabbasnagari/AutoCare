using AutoCare.MVVM;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AutoCare.Models
{
    public class SaleRecord : ViewModelBase
    {

        public int Id { get; set; }
        public string Customer { get; set; }

        public ObservableCollection<SaleItem> Items { get; set; } = new ObservableCollection<SaleItem>();

        public SaleRecord()
        {
            Items.CollectionChanged += (sender, args) =>
            {
                OnPropertyChanged();
                OnPropertyChanged(nameof(SubTotal));
                OnPropertyChanged(nameof(Total));
            };
        }

        public DateTime Date { get; set; }

        public double SubTotal => Items.Sum(item => item.TotalPrice);

        private double tax;
        public double Tax
        {
            get => tax;
            set
            {
                if (tax != value)
                {
                    tax = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        private double discount;
        public double Discount
        {
            get => discount;
            set
            {
                if (discount != value)
                {
                    discount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public double Total => SubTotal + Tax - Discount;

        public string? Status { get; set; }

        private double receivedAmount;
        public double ReceivedAmount
        {
            get => receivedAmount;
            set
            {
                if (receivedAmount != value)
                {
                    receivedAmount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Change));
                }
            }
        }

        public double Change => ReceivedAmount - Total;
    }

}
