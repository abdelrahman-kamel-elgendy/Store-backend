using Store.Core.Entities.Product;
using Store.Core.Interfaces;
using Store.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure.Repositories
{
    public class PhotoRepossitory:GenericRepository<Photo>, IPhotoRepository
    {
        public PhotoRepossitory(AppDbContext context) : base(context)
        {
        }
    }
}
