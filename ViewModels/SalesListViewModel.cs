using InventoryApp.Data;
using InventoryApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels
{
    public class SalesListViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        public ObservableCollection<Sale> Sales { get; set; } = new();

        public SalesListViewModel()
        {
            _context = new AppDbContext();  
            LoadSales();
        }

        private void LoadSales()
        {
            var sales = _context.Sales
                .Include(s => s.ProductSales)
                .ThenInclude(ps => ps.Product)
                .ToList();

            Sales.Clear();
            foreach (var sale in sales)
                Sales.Add(sale);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
