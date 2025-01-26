using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCare.Models
{
    public class SaleItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

        private int quantity;
        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(Quantity));
            }
        }
        public double Price { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Debug.WriteLine(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double TotalPrice => Quantity * Price;

        public event PropertyChangedEventHandler? PropertyChanged;

        public override string ToString()
        {
            return $"{ItemId} (x{Quantity})   {Price}   {TotalPrice}";
        }
    }
}
