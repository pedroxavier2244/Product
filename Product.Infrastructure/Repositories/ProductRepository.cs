using Microsoft.EntityFrameworkCore;
using Product.Domain.Interfaces;
using Product.Infrastructure.Data;

namespace Product.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Domain.Models.Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Delete(Domain.Models.Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<IEnumerable<Domain.Models.Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Domain.Models.Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Update(Domain.Models.Product product)
        {
            _context.Products.Update(product);
        }
    }
}