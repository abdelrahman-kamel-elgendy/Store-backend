using Store.Core.Interfaces;
using Store.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICategoryRepository CategoryRepository { get; }

        public IPhotoRepository PhotoRepository { get; }

        public IProductRepository ProductRepository { get; }


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(_context);
            PhotoRepository = new PhotoRepossitory(_context);
            ProductRepository = new ProductRepository(_context);

        }
    }
}
