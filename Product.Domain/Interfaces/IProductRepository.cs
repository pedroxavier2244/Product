using ProductEntity = Product.Domain.Models.Product;

namespace Product.Domain.Interfaces
{
    public interface IProductRepository
    {

        Task<ProductEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductEntity>> GetAllAsync();
        Task AddAsync(ProductEntity product);
        void Update(ProductEntity product);
        void Delete(ProductEntity product);
        Task<int> SaveChangesAsync();
    }
}