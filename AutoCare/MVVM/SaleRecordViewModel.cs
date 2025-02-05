using System.Collections.ObjectModel;
using System.ComponentModel;
using AutoCare.Models;

namespace AutoCare.MVVM
{
    public class SaleRecordViewModel : ViewModelBase
    {
        private int Id;
        public ObservableCollection<SaleItem> Items { get; } = new ObservableCollection<SaleItem>();

        public SaleRecordViewModel()
        {
            _customer = "N/A";
            Items.CollectionChanged += (s, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (SaleItem item in e.OldItems)
                    {
                        item.PropertyChanged -= SaleItem_PropertyChanged;
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (SaleItem item in e.NewItems)
                    {
                        item.PropertyChanged += SaleItem_PropertyChanged;
                    }
                }
                OnPropertyChanged(nameof(SubTotal), nameof(Total));
            };
        }

        private void SaleItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SaleItem.TotalPrice))
            {
                OnPropertyChanged(nameof(SubTotal), nameof(Total));
            }
        }

        private string _customer;
        public string Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public double SubTotal => Items.Sum(i => i.TotalPrice);


        private double _tax;
        public double Tax
        {
            get => _tax;
            set => SetProperty(ref _tax, value, nameof(Tax), nameof(Total), nameof(ReturnChange));
        }

        public double _discount;
        public double Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value, nameof(Discount), nameof(Total), nameof(ReturnChange));
        }

        public double Total => (SubTotal + _tax) - _discount;

        private string? _status;
        public string? Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private double _receivedAmount;
        public double ReceivedAmount
        {
            get => _receivedAmount;
            set => SetProperty(ref _receivedAmount, value, nameof(ReceivedAmount), nameof(ReturnChange));
        }

        public double ReturnChange => _receivedAmount - Total;
    }
}
