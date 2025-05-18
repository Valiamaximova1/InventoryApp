using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using InventoryApp.Models;

namespace InventoryApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<ProductSale> ProductSales => Set<ProductSale>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductSale>()
                .HasKey(ps => new { ps.ProductId, ps.SaleId });

            modelBuilder.Entity<ProductSale>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSales)
                .HasForeignKey(ps => ps.ProductId);

            modelBuilder.Entity<ProductSale>()
                .HasOne(ps => ps.Sale)
                .WithMany(s => s.ProductSales)
                .HasForeignKey(ps => ps.SaleId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;port=3306;database=inventorydb;user=db;password=12345678;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public void SeedDatabase()
        {
            if (!Suppliers.Any())
            {
                Suppliers.AddRange(
                    new Supplier { Name = "Метро" },
                    new Supplier { Name = "Булмаг" },
                    new Supplier { Name = "Верея" },
                    new Supplier { Name = "Дерони" }
                );
            }

            if (!Categories.Any())
            {
                Categories.AddRange(
                    new Category { Name = "Месо" },
                    new Category { Name = "Млечни" },
                    new Category { Name = "Плодове" },
                    new Category { Name = "Зеленчуци" }
                );
            }

            SaveChanges();

            if (!Products.Any())
            {
                var metro = Suppliers.First(s => s.Name == "Метро");
                var mlechni = Categories.First(c => c.Name == "Млечни");

                var product1 = new Product
                {
                    Name = "Кисело мляко",
                    Price = 2,
                    Quantity = 100,
                    SupplierId = metro.Id
                };
                Products.Add(product1);
                SaveChanges();

                ProductCategories.Add(new ProductCategory
                {
                    ProductId = product1.Id,
                    CategoryId = mlechni.Id
                });

                SaveChanges();
            }
        }

    }
}
