using AutoMapper;
using Microsoft.Extensions.Logging;
using Product.Application.DTOs;
using Product.Application.Interfaces;
using Product.Domain.Interfaces;


namespace Product.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IMapper mapper, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        } 

        public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
        {
            var product = new Domain.Models.Product(
                createProductDto.Name,
                createProductDto.Description,
                createProductDto.Price,
                createProductDto.Stock
            );

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                _productRepository.Delete(product);
                await _productRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateAsync(Guid id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                product.Update(
                    updateProductDto.Name,
                    updateProductDto.Description,
                    updateProductDto.Price,
                    updateProductDto.Stock
                );

                _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();
            }
        }
    }
}