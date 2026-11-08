using Store.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Core.Interfaces
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        // Add any additional methods specific to Product repository if needed..
    }
}
