using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        public override bool Equals(object? obj)
        {
            return obj is Category category && category.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }

}
