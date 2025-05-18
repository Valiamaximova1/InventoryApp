using InventoryApp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace InventoryApp.ViewModels
{
    public class AddEditSupplierViewModel : INotifyPropertyChanged
    {
        private Window? _window;

        public Supplier Supplier { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadedCommand { get; }

        public AddEditSupplierViewModel(Supplier supplier)
        {
            Supplier = supplier;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            LoadedCommand = new RelayCommand(AttachWindow);
        }

        private void AttachWindow()
        {
            if (_window != null) return;

            _window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Supplier.Name))
            {
                MessageBox.Show("Моля, въведи име на доставчик.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _window.DialogResult = true;
            _window.Close();
        }

        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
