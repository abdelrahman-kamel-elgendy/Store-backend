using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Core.Entities.Product
{
    /// <summary>A product listed in the store catalog.</summary>
    public class Product:BaseEntity<int>
    {
        /// <summary>Gets or sets the product's display name.</summary>
        public String Name { get; set; }

        /// <summary>Gets or sets the product description.</summary>
        public String Description { get; set; }

        /// <summary>Gets or sets the product price.</summary>
        public decimal Price { get; set; }

        /// <summary>Gets or sets the product's associated photos.</summary>
        public ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();

        /// <summary>Gets or sets the category foreign key.</summary>
        public int CategoryId { get; set; }

        /// <summary>Gets or sets the category that contains this product.</summary>
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }
    }
}
