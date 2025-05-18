using InventoryApp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
                if (_quantity != value && value > 0)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public decimal TotalPrice { 
            get =>  Product != null ? Product.Price * Quantity  : 0;
            set { }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
