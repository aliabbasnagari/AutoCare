using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AutoCare.Models;

namespace AutoCare.MVVM
{
    public class SaleRecordViewModel : ViewModelBase
    {
        private SaleRecord _saleRecord;
        public ObservableCollection<SaleItem> Items { get; }

        public SaleRecordViewModel(SaleRecord? saleRecord = null)
        {
            _saleRecord = saleRecord ?? new SaleRecord();
            Items = new ObservableCollection<SaleItem>(_saleRecord.Items);
            foreach (var item in Items)
            {
                item.PropertyChanged += SaleItem_PropertyChanged;
            }
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void SaleItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SaleItem.TotalPrice))
            {
                OnPropertyChanged(nameof(SubTotal), nameof(Total));
            }
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
            OnPropertyChanged(nameof(SubTotal), nameof(Total), nameof(ReturnChange));
        }

        public int Id
        {
            get => _saleRecord.Id;
            set
            {
                if (_saleRecord.Id != value)
                {
                    _saleRecord.Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Customer
        {
            get => _saleRecord.Customer;
            set
            {
                if (_saleRecord.Customer != value)
                {
                    _saleRecord.Customer = value;
                    OnPropertyChanged();
                }
            }
        }


        public DateTime Date
        {
            get => _saleRecord.Date;
            set
            {
                if (_saleRecord.Date != value)
                {
                    _saleRecord.Date = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SubTotal => _saleRecord.SubTotal;

        public double Tax
        {
            get => _saleRecord.Tax;
            set
            {
                if (_saleRecord.Tax != value)
                {
                    _saleRecord.Tax = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total), nameof(ReturnChange));
                }
            }
        }

        public double Discount
        {
            get => _saleRecord.Discount;
            set
            {
                if (_saleRecord.Discount != value)
                {
                    _saleRecord.Discount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total));
                    OnPropertyChanged(nameof(ReturnChange));
                }
            }
        }

        public double Total => _saleRecord.Total;

        public string? Status
        {
            get => _saleRecord.Status;
            set
            {
                if (_saleRecord.Status != value)
                {
                    _saleRecord.Status = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ReceivedAmount
        {
            get => _saleRecord.ReceivedAmount;
            set
            {
                if (_saleRecord.ReceivedAmount != value)
                {
                    _saleRecord.ReceivedAmount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ReturnChange));
                }
            }
        }

        public double ReturnChange => _saleRecord.ReturnChange;
    }
}
