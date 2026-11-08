using Store.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Core.Interfaces
{
    public interface ICategoryRepository:IGenericRepository<Category>
    {
        // Add any additional methods specific to Category repository if needed..
    }
}
