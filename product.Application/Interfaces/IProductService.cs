using Product.Application.DTOs;

namespace Product.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
        Task UpdateAsync(Guid id, UpdateProductDto updateProductDto);
        Task DeleteAsync(Guid id);
    }
}