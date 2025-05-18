using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using InventoryApp.Data;
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

        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }

        public ICommand ClearCartCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand CheckoutCommand { get; }

        private CartViewModel() {
            IncreaseQuantityCommand = new RelayCommand<CartItem>(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand<CartItem>(DecreaseQuantity);
            ClearCartCommand = new RelayCommand(ClearCart, () => CartItems.Any());
            RemoveItemCommand = new RelayCommand<CartItem>(RemoveItem);
            CheckoutCommand = new RelayCommand(Checkout, () => CartItems.Any());

            CartItems.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalQuantity));
                OnPropertyChanged(nameof(TotalPrice));
                (ClearCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CheckoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
            };
        }
        public int TotalQuantity => CartItems.Sum(ci => ci.Quantity);
        public decimal TotalPrice => CartItems.Sum(ci => ci.TotalPrice);

        private void Checkout()
        {
            using var context = new AppDbContext();

            var sale = new Sale { 
            Date = DateTime.Now};

            foreach (var item in CartItems)
            {
                var product = context.Products.FirstOrDefault(p => p.Id == item.Product.Id);
                if (product == null || product.Quantity < item.Quantity)
                {
                    MessageBox.Show($"Няма достатъчно наличност за {item.Product.Name}.");
                    return;
                }

                product.Quantity -= item.Quantity;

                sale.ProductSales.Add(new ProductSale
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            context.Sales.Add(sale);
            context.SaveChanges();

            MessageBox.Show("Поръчката е успешно записана!");
            //ClearCart();
            CartItems.Clear();
        }



        public void AddOrUpdateItem(Product product)
        {
            var existing = CartItems.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing != null)
            {
                existing.Quantity++;
              
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    Product = product,
                    Quantity = 1,
              
                });
            }

            OnPropertyChanged(nameof(CartItems));
        }
        private void DecreaseQuantity(CartItem item)
        {
            if (item.Quantity > 1)
                item.Quantity--;
            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(TotalPrice));
        }

        private void IncreaseQuantity(CartItem item)
        {
            if (item.Quantity < item.Product.Quantity)
                item.Quantity++;
            else
                MessageBox.Show("Няма достатъчна наличност от продукта.");
            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(TotalPrice));
        }

        private void ClearCart()
        {
            if (CartItems.Any())
            {
                var result = MessageBox.Show("Сигурни ли сте, че искате да изчистите количката?",
                                             "Потвърждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    CartItems.Clear();
            }
        }

        private void RemoveItem(CartItem item)
        {
            if (item != null)
                CartItems.Remove(item);
        }

    }
}