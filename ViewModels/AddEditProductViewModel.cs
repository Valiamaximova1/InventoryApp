using System;
using System.Collections.Generic;
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
    public class AddEditProductViewModel : INotifyPropertyChanged
    {
      

        public Product Product { get; set; }

        public ObservableCollection<Supplier> Suppliers { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Category> SelectedCategories { get; set; } = new();
        private Window? _window;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadedCommand { get; }

        public AddEditProductViewModel(Product product)
        {
            Product = product;

            using var context = new AppDbContext();
            var allCategories = context.Categories.ToList();
            Categories = new ObservableCollection<Category>(allCategories);

            Categories = new ObservableCollection<Category>(context.Categories.ToList());
            SelectedCategories = new ObservableCollection<Category>();

            if (Product.ProductCategories != null)
            {
                var selectedIds = Product.ProductCategories.Select(pc => pc.CategoryId).ToHashSet();

                foreach (var category in Categories)
                {
                    if (selectedIds.Contains(category.Id))
                        SelectedCategories.Add(category);   }
            }

            Suppliers = new ObservableCollection<Supplier>(context.Suppliers.ToList());

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            LoadedCommand = new RelayCommand(AttachWindow);
        }



        public void AttachWindow()
        {
            if (_window != null) return;

            _window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Моля, въведи име на продукта.");
                return;
            }
        

            if (Price <= 0)
            {
                MessageBox.Show("Цената трябва да е положително число.");
                return;
            }

            if (Quantity < 0)
            {
                MessageBox.Show("Количеството не може да е отрицателно.");
                return;
            }

            if (SupplierId == 0)
            {
                MessageBox.Show("Моля, избери доставчик.");
                return;
            }

            if ( SelectedCategories.Count == 0)
            {
                MessageBox.Show("Моля, избери поне една категория.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Product.ProductCategories = SelectedCategories
                .Select(c => new ProductCategory { CategoryId = c.Id, ProductId = Product.Id })
                .ToList();


            _window.DialogResult = true;
            _window.Close();
        }



        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
        }

        public string Name
        {
            get => Product.Name;
            set
            {
                if (Product.Name != value)
                {
                    Product.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Price
        {
            get => Product.Price;
            set
            {
                if (Product.Price != value)
                {
                    Product.Price = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Quantity
        {
            get => Product.Quantity;
            set
            {
                if (Product.Quantity != value)
                {
                    Product.Quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SupplierId
        {
            get => Product.SupplierId;
            set
            {
                if (Product.SupplierId != value)
                {
                    Product.SupplierId = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
