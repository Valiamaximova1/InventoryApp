using InventoryApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace InventoryApp.ViewModels
{
    public class AddEditCategoryViewModel : INotifyPropertyChanged
    {
        public Category Category { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadedCommand { get; }

        private Window? _window;

        public AddEditCategoryViewModel(Category category)
        {
            Category = category;
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            LoadedCommand = new RelayCommand(AttachWindow);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Category.Name))
            {
                MessageBox.Show("Моля, въведете име на категорията.");
                return;
            }
         _window!.DialogResult = true;
            _window?.Close();
          
        }

        private void Cancel()
        { 
            _window!.DialogResult = false;
            _window?.Close();
        
        }


        public void AttachWindow()
        {
            if (_window != null) return;

            _window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
