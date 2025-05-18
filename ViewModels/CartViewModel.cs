using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using InventoryApp.Models;

namespace InventoryApp.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
       
        private static CartViewModel _instance;
        public static CartViewModel Instance => _instance ??= new CartViewModel();

        public ObservableCollection<CartItem> CartItems { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private CartViewModel() { }

        public void AddOrUpdateItem(Product product)
        {
            var existing = CartItems.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing != null)
            {
                existing.Quantity++;
                existing.TotalPrice = existing.Quantity * existing.Product.Price;
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    Product = product,
                    Quantity = 1,
                    TotalPrice = product.Price
                });
            }

            OnPropertyChanged(nameof(CartItems));
        }

        public void ClearCart()
        {
            CartItems.Clear();
            OnPropertyChanged(nameof(CartItems));
        }
    }
}