using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using InventoryApp.Views;

namespace InventoryApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand ShowProductsCommand { get; }
        public ICommand ShowCategoriesCommand { get; }
        public ICommand ShowSupplierCommand { get; }
        public ICommand ShowCartCommand { get; }
        public ICommand SalesListCommand { get; }
        public MainWindowViewModel()
        {
            ShowProductsCommand = new RelayCommand( ShowProducts);
            ShowCategoriesCommand = new RelayCommand(ShowCategories);
            ShowSupplierCommand = new RelayCommand(ShowSupplier);
            ShowCartCommand = new RelayCommand(ShowCart);
            SalesListCommand = new RelayCommand(AllSales);

            ShowProducts(); 
        }

        private void ShowProducts()
        {
            CurrentView = new ProductListView();
        }

        private void ShowSupplier()
        {
            CurrentView = new SupplierListView();
        }

        private void ShowCart()
        {
            CurrentView = new CartView();
        }
        private void AllSales()
        {
            CurrentView = new SalesListView();
        }

        private void ShowCategories()
        {
            CurrentView = new CategoryListView();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}