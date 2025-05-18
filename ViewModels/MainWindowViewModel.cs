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

        public MainWindowViewModel()
        {
            ShowProductsCommand = new RelayCommand( ShowProducts);
            ShowCategoriesCommand = new RelayCommand(ShowCategories);

            ShowProducts(); // Начален изглед
        }

        private void ShowProducts()
        {
            CurrentView = new ProductListView();
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