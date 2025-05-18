using InventoryApp.Data;
using InventoryApp.Models;
using InventoryApp.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InventoryApp.ViewModels
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private Product _selectedProduct;
         private string _quantitySortOrder = "Без сортиране";

      

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Supplier> Suppliers { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        private ObservableCollection<Product> _selectedProducts = new();

        private string _searchText = string.Empty;

        private Supplier? _selectedSupplier;   
        private Category? _selectedCategory;

        public ICommand AddCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand AddToCartCommand { get; }


        public ProductViewModel()
        {
            _context = new AppDbContext();
            LoadSuppliers();
            LoadCategories();
            LoadProducts();

            SelectedProducts.CollectionChanged += SelectedProductsChanged;

            LoadCommand = new RelayCommand(LoadProducts);
            SearchCommand = new RelayCommand(ApplyFilters);
            AddCommand = new RelayCommand(AddProduct);
            EditCommand = new RelayCommand(EditProduct);
            DeleteCommand = new RelayCommand(DeleteSelectedProducts, () => SelectedProducts.Any());
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            AddToCartCommand = new RelayCommand<Product>(AddToCart);


        }

        public ObservableCollection<Product> SelectedProducts
        {
            get => _selectedProducts;
            set
            {
                if (_selectedProducts != null)
                    _selectedProducts.CollectionChanged -= SelectedProductsChanged;

                _selectedProducts = value;

                if (_selectedProducts != null)
                    _selectedProducts.CollectionChanged += SelectedProductsChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDelete));
            }
        } 
        public bool CanEdit => SelectedProducts.Count == 1;
        public bool CanDelete => SelectedProducts.Count > 0;

        private void SelectedProductsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(CanDelete));
            (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();

            if (SelectedProducts.Count == 1)
                SelectedProduct = SelectedProducts.First();
            else
                SelectedProduct = null;
        }

        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private void AddToCart(Product product)
        {
            if (product.Quantity <= 0)
            {
                MessageBox.Show("Продуктът е изчерпан!", "Няма наличност", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CartViewModel.Instance.AddOrUpdateItem(product);
        }
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            var query = _context.Products
                .Include(p => p.Supplier)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var lowerSearch = SearchText.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(lowerSearch));
            }

            if (SelectedSupplier != null && SelectedSupplier.Id != 0)
            {
                query = query.Where(p => p.SupplierId == SelectedSupplier.Id);
            }

            if (SelectedCategory != null && SelectedCategory.Id != 0)
            {
                query = query.Where(p =>
                    p.ProductCategories.Any(pc => pc.CategoryId == SelectedCategory.Id));
            }

            switch (QuantitySortOrder)
            {
                case "Възходящ":
                    query = query.OrderBy(p => p.Quantity);
                    break;
                case "Низходящ":
                    query = query.OrderByDescending(p => p.Quantity);
                    break;
            }


            Products.Clear();
            foreach (var product in query.ToList())
                Products.Add(product);
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedSupplier = Suppliers.FirstOrDefault();   
            SelectedCategory = Categories.FirstOrDefault();
            QuantitySortOrder = "Без сортиране";
            ApplyFilters();
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private void AddProduct()
        {
            var newProduct = new Product();
            var viewModel = new AddEditProductViewModel(newProduct);
            var window = new AddEditProductWindow { DataContext = viewModel };

            var result = window.ShowDialog();
            if (result == true)
            {
                _context.Products.Add(viewModel.Product);
                _context.SaveChanges();
                LoadProducts();
            }
        }

        private void EditProduct()
        {
            if (SelectedProduct == null)
                return;

            var editableProduct = new Product
            {
                Id = SelectedProduct.Id,
                Name = SelectedProduct.Name,
                Price = SelectedProduct.Price,
                Quantity = SelectedProduct.Quantity,
                SupplierId = SelectedProduct.SupplierId,
                ProductCategories = SelectedProduct.ProductCategories?.Select(pc => new ProductCategory
                {
                    ProductId = pc.ProductId,
                    CategoryId = pc.CategoryId
                }).ToList()
            };

            var viewModel = new AddEditProductViewModel(editableProduct);
            var window = new AddEditProductWindow { DataContext = viewModel };

            var result = window.ShowDialog();
            if (result == true)
            {
                SelectedProduct.Name = editableProduct.Name;
                SelectedProduct.Price = editableProduct.Price;
                SelectedProduct.Quantity = editableProduct.Quantity;
                SelectedProduct.SupplierId = editableProduct.SupplierId;
                SelectedProduct.ProductCategories = editableProduct.ProductCategories;

                _context.Products.Update(SelectedProduct);
                _context.SaveChanges();
                LoadProducts();
                SelectedProduct = Products.FirstOrDefault(p => p.Id == editableProduct.Id);
            }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }

    
      

        private void LoadSuppliers()
        {
            Suppliers.Clear();
            Suppliers.Add(new Supplier { Id = 0, Name = "Всички доставчици" });
            var list = _context.Suppliers.ToList();
            foreach (var supplier in list)
                Suppliers.Add(supplier);
            SelectedSupplier = Suppliers.FirstOrDefault();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            Categories.Add(new Category { Id = 0, Name = "Всички категории" });
            var list = _context.Categories.ToList();
            foreach (var category in list)
                Categories.Add(category);
            SelectedCategory = Categories.FirstOrDefault();
        }

        private void DeleteSelectedProducts()
        {
            if (!SelectedProducts.Any()) return;

            string productNames = string.Join(", ", SelectedProducts.Select(p => p.Name));
            var result = MessageBox.Show(
                $"Сигурни ли сте, че искате да изтриете следните продукти:\n{productNames}?",
                "Потвърждение за изтриване",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _context.Products.RemoveRange(SelectedProducts);
                _context.SaveChanges();
                LoadProducts();
                SelectedProducts.Clear();
            }
        }


        private void LoadProducts()
        {
            Products.Clear();
            var items = _context.Products
                .Include(p => p.Supplier)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .ToList();

            foreach (var product in items)
                Products.Add(product);
            OnPropertyChanged(nameof(Products));
        }

      
        public string QuantitySortOrder
        {
            get => _quantitySortOrder;
            set
            {
                _quantitySortOrder = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public List<string> QuantitySortOptions { get; } = new()
        {
            "Без сортиране",
            "Възходящ",
            "Низходящ"
        };

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
