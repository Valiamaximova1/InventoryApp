using InventoryApp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace InventoryApp.Models
{
    public class CartItem : INotifyPropertyChanged
    {
        public Product Product { get; set; }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value > Product.Quantity)
                {
                    MessageBox.Show($"Налично количество: {Product.Quantity}", "Недостатъчна наличност", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(RemainingQuantity));
            }
        }

        public int RemainingQuantity => Product.Quantity - Quantity;

        public decimal TotalPrice { 
            get =>  Product != null ? Product.Price * Quantity  : 0;
            set { }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
