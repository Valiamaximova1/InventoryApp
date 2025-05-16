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
        private bool _canEdit;

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Supplier> Suppliers { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        private string _searchText = string.Empty;

        private Supplier? _selectedSupplier;   
        private Category? _selectedCategory;

        public ICommand AddCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ClearFiltersCommand { get; }


        public ProductViewModel()
        {
            _context = new AppDbContext();
            LoadSuppliers();
            LoadCategories();
            LoadCommand = new RelayCommand(LoadProducts);
            SearchCommand = new RelayCommand(ApplyFilters);
            AddCommand = new RelayCommand(AddProduct);
            EditCommand = new RelayCommand(EditProduct);
            ClearFiltersCommand = new RelayCommand(ClearFilters);


            LoadProducts();
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

            Products.Clear();
            foreach (var product in query.ToList())
                Products.Add(product);
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedSupplier = Suppliers.FirstOrDefault();   
            SelectedCategory = Categories.FirstOrDefault();   
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

        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                _canEdit = value;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
