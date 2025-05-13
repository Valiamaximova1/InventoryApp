
using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace InventoryApp.ViewModels
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public string SearchText { get; set; } = string.Empty;

        public ICommand LoadCommand { get; }
        public ICommand SearchCommand { get; }

        public ProductViewModel()
        {
            _context = new AppDbContext();

            LoadCommand = new RelayCommand(LoadProducts);
            SearchCommand = new RelayCommand(SearchProducts);

            LoadProducts();
        }

        private void LoadProducts()
        {
            Products.Clear();
            var items = _context.Products.Include(p => p.Supplier).ToList();

            foreach (var product in items)
                Products.Add(product);
        }

        private void SearchProducts()
        {
            Products.Clear();
            var filtered = _context.Products
                .Include(p => p.Supplier)
                .Where(p => p.Name.Contains(SearchText))
                .ToList();

            foreach (var product in filtered)
                Products.Add(product);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

