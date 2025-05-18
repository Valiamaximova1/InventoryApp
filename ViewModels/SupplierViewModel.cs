using InventoryApp.Data;
using InventoryApp.Models;
using InventoryApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace InventoryApp.ViewModels
{
    public class SupplierViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Supplier> Suppliers { get; set; } = new();
        public ObservableCollection<Supplier> SelectedSuppliers { get; set; } = new();

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public bool CanEdit => SelectedSuppliers.Count == 1;
        public bool CanDelete => SelectedSuppliers.Count > 0;

        public SupplierViewModel()
        {
            _context = new AppDbContext();
            LoadSuppliers();

            AddCommand = new RelayCommand(AddSupplier);
            EditCommand = new RelayCommand(EditSupplier);
            DeleteCommand = new RelayCommand(DeleteSupplier);

            SelectedSuppliers.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDelete));
            };
        }

        private void LoadSuppliers()
        {
            Suppliers.Clear();
            foreach (var supplier in _context.Suppliers.ToList())
                Suppliers.Add(supplier);
        }

        private void AddSupplier()
        {
            var newSupplier = new Supplier();
            var vm = new AddEditSupplierViewModel(newSupplier);
            var window = new AddEditSupplierWindow { DataContext = vm };

            if (window.ShowDialog() == true)
            {
                _context.Suppliers.Add(vm.Supplier);
                _context.SaveChanges();
                LoadSuppliers();
            }
        }

        private void EditSupplier()
        {
            if (!CanEdit) return;

            var toEdit = SelectedSuppliers.First();

            var editable = new Supplier
            {
                Id = toEdit.Id,
                Name = toEdit.Name
            };

            var vm = new AddEditSupplierViewModel(editable);
            var window = new AddEditSupplierWindow { DataContext = vm };

            if (window.ShowDialog() == true)
            {
                toEdit.Name = editable.Name;
                _context.Suppliers.Update(toEdit);
                _context.SaveChanges();
                LoadSuppliers();
            }
        }

        private void DeleteSupplier()
        {
            if (!CanDelete) return;

            var names = string.Join(", ", SelectedSuppliers.Select(s => s.Name));
            var result = MessageBox.Show(
                $"Сигурни ли сте, че искате да изтриете: {names}?",
                "Изтриване", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _context.Suppliers.RemoveRange(SelectedSuppliers);
                _context.SaveChanges();
                LoadSuppliers();
                SelectedSuppliers.Clear();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
