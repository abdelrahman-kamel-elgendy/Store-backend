using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Core.Entities.Product
{
    public class Category:BaseEntity<int>
    {
        public String Name { get; set; }
        public String Description { get; set; }

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
