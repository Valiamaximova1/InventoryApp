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
                    new Supplier { Name = "Дерони" },
                    new Supplier { Name = "Тандем" }
                );
            }

            if (!Categories.Any())
            {
                Categories.AddRange(
                    new Category { Name = "Месо и месни продукти" },
                    new Category { Name = "Млечни продукти" },
                    new Category { Name = "Консервирани храни" },
                    new Category { Name = "Плодове" },
                    new Category { Name = "Зеленчуци" },
                    new Category { Name = "Напитки" }
                );
            }

            SaveChanges();

            if (!Products.Any())
            {
                var metro = Suppliers.First(s => s.Name == "Метро");
                var bulmag = Suppliers.First(s => s.Name == "Булмаг");
                var vereq = Suppliers.First(s => s.Name == "Верея");
                var deroni = Suppliers.First(s => s.Name == "Дерони");
                var tandem = Suppliers.First(s => s.Name == "Тандем");
                var meso = Categories.First(c => c.Name == "Месо и месни продукти");
                var mlechni = Categories.First(c => c.Name == "Млечни продукти");
                var konservi = Categories.First(c => c.Name == "Консервирани храни");
                var plodove = Categories.First(c => c.Name == "Плодове");
                var zelenchuci = Categories.First(c => c.Name == "Зеленчуци");
                var napitki = Categories.First(c => c.Name == "Напитки");

                var product1 = new Product
                {
                    Name = "Кашкавал Верея",
                    Price = 7,
                    Quantity = 10,
                    SupplierId = metro.Id
                };
                var product2 = new Product
                            {
                                Name = "Лютеница",
                                Price = 3,
                                Quantity = 100,
                                SupplierId = deroni.Id
                            };

                var product3 = new Product
                {
                    Name = "Розови домати",
                    Price = 4,
                    Quantity = 120,
                    SupplierId = vereq.Id
                };
                var product4 = new Product
                {
                    Name = "Свински врат",
                    Price = 13,
                    Quantity = 12,
                    SupplierId = tandem.Id
                };
                var product5 = new Product
                {
                    Name = "Минерална вода",
                    Price = 1,
                    Quantity = 9,
                    SupplierId = bulmag.Id
                };
                Products.Add(product1);
                Products.Add(product2);
                Products.Add(product3);
                Products.Add(product4);
                Products.Add(product5);
                SaveChanges();

                ProductCategories.Add(new ProductCategory
                {
                    ProductId = product1.Id,
                    CategoryId = mlechni.Id,
                });
                ProductCategories.Add(new ProductCategory
                {
                    ProductId = product2.Id,
                    CategoryId = konservi.Id,
                });
                ProductCategories.Add(new ProductCategory
                {
                    ProductId = product3.Id,
                    CategoryId = plodove.Id,
                });
                ProductCategories.Add(new ProductCategory
                {
                    ProductId = product3.Id,
                    CategoryId = zelenchuci.Id,
                });
                ProductCategories.Add(new ProductCategory
                {
                    ProductId = product4.Id,
                    CategoryId = meso.Id,
                });
                ProductCategories.Add(new ProductCategory
                {
                    ProductId = product5.Id,
                    CategoryId = napitki.Id,
                });

                SaveChanges();
            }
        }

    }
}
