using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Core.Entities.Product
{
    public class Product:BaseEntity<int>
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public decimal Price { get; set; }

        public ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }
    }
}
