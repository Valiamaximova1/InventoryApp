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
    public class CategoryViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Category> SelectedCategories { get; set; } = new();

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public bool CanEdit => SelectedCategories.Count == 1;
        public bool CanDelete => SelectedCategories.Count > 0;

        public CategoryViewModel()
        {
            _context = new AppDbContext();
            LoadCategories();

            AddCommand = new RelayCommand(AddCategory);
            EditCommand = new RelayCommand(EditCategory);
            DeleteCommand = new RelayCommand(DeleteCategory);

            SelectedCategories.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDelete));
            };
        }

        private void LoadCategories()
        {
            Categories.Clear();
            foreach (var category in _context.Categories.ToList())
                Categories.Add(category);
        }

        private void AddCategory()
        {
            var newCategory = new Category();
            var vm = new AddEditCategoryViewModel(newCategory);
            var window = new AddEditCategoryWindow { DataContext = vm };

            if (window.ShowDialog() == true)
            {
                _context.Categories.Add(vm.Category);
                _context.SaveChanges();
                LoadCategories();
            }
        }

        private void EditCategory()
        {
            if (SelectedCategories.Count != 1) return;

            var toEdit = SelectedCategories.First();

            var editable = new Category
            {
                Id = toEdit.Id,
                Name = toEdit.Name
            };

            var vm = new AddEditCategoryViewModel(editable);
            var window = new AddEditCategoryWindow { DataContext = vm };

            if (window.ShowDialog() == true)
            {
                toEdit.Name = editable.Name;
                _context.Categories.Update(toEdit);
                _context.SaveChanges();
                LoadCategories();
            }
        }

        private void DeleteCategory()
        {
            if (SelectedCategories.Count == 0) return;

            var names = string.Join(", ", SelectedCategories.Select(c => c.Name));
            var result = MessageBox.Show(
                $"Сигурни ли сте, че искате да изтриете: {names}?",
                "Изтриване", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _context.Categories.RemoveRange(SelectedCategories);
                _context.SaveChanges();
                LoadCategories();
                SelectedCategories.Clear();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
