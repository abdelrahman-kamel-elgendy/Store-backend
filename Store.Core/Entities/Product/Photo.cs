using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Store.Core.Entities.Product
{
    public class Photo:BaseEntity<int>
    {
        public String Name { get; set; }
        public String Url { get; set; }
        public bool IsMain { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        

    }
}
