using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Core.Entities.Product
{
    /// <summary>An image associated with a product.</summary>
    public class Photo:BaseEntity<int>
    {
        /// <summary>Gets or sets the photo's display name.</summary>
        public String Name { get; set; }

        /// <summary>Gets or sets the photo URL.</summary>
        public String Url { get; set; }

        /// <summary>Gets or sets whether this is the product's main photo.</summary>
        public bool IsMain { get; set; }

        /// <summary>Gets or sets the product foreign key.</summary>
        public int ProductId { get; set; }

        /// <summary>Gets or sets the product associated with this photo.</summary>
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        

    }
}
