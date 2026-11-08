using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Core.Interfaces
{
    public interface IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public IPhotoRepository PhotoRepository{ get; }
        public IProductRepository ProductRepository { get; }

    }
}
