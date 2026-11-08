using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Core.Entities.Product
{
    /// <summary>A catalog category that groups products.</summary>
    public class Category:BaseEntity<int>
    {
        /// <summary>Gets or sets the category's display name.</summary>
        public String Name { get; set; }

        /// <summary>Gets or sets the category description.</summary>
        public String Description { get; set; }

        /// <summary>Gets or sets the products assigned to this category.</summary>
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
