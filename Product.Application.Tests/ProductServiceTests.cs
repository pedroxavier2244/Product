using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Product.Application.DTOs;
using Product.Application.Services;
using Product.Domain.Interfaces;
using ProductEntity = Product.Domain.Models.Product;

namespace Product.Application.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _productService;

      
        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ProductService>>();

           
            _productService = new ProductService(
                _productRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

      

        [Fact]
        public async Task GetByIdAsync_WhenProductExists_ShouldReturnProductDto()
        {
          
            var productId = Guid.NewGuid();
            var fakeProduct = new ProductEntity("Test Product", "Description", 100, 10);
            var fakeProductDto = new ProductDto(productId, "Test Product", "Description", 100, 10, DateTime.UtcNow);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(fakeProduct);
            _mapperMock.Setup(m => m.Map<ProductDto>(fakeProduct)).Returns(fakeProductDto);

        
            var result = await _productService.GetByIdAsync(productId);

            
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(fakeProductDto);
            _productRepositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
        {
           
            var productId = Guid.NewGuid();
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((ProductEntity)null);

            
            var result = await _productService.GetByIdAsync(productId);

           
            result.Should().BeNull();
        }

        

        [Fact]
        public async Task GetAllAsync_WhenProductsExist_ShouldReturnListOfProductDtos()
        {
            
            var fakeProducts = new List<ProductEntity>
            {
                new ProductEntity("Product 1", "Desc 1", 10, 1),
                new ProductEntity("Product 2", "Desc 2", 20, 2)
            };
            var fakeProductDtos = new List<ProductDto>
            {
                new ProductDto(Guid.NewGuid(), "Product 1", "Desc 1", 10, 1, DateTime.UtcNow),
                new ProductDto(Guid.NewGuid(), "Product 2", "Desc 2", 20, 2, DateTime.UtcNow)
            };

            _productRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(fakeProducts);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(fakeProducts)).Returns(fakeProductDtos);

            
            var result = await _productService.GetAllAsync();

            
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(fakeProductDtos);
        }

       

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCallAddAndSaveChangesAndReturnDto()
        {
            
            var createDto = new CreateProductDto("New Product", "New Desc", 150, 50);

          

            var createdProductDto = new ProductDto(Guid.NewGuid(), createDto.Name, createDto.Description, createDto.Price, createDto.Stock, DateTime.UtcNow);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<ProductEntity>())).Returns(createdProductDto);

            var result = await _productService.CreateAsync(createDto);

            
            result.Should().BeEquivalentTo(createdProductDto);
            _productRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ProductEntity>()), Times.Once);
            _productRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

      

        [Fact]
        public async Task UpdateAsync_WhenProductExists_ShouldCallUpdateAndSaveChanges()
        {
            
            var productId = Guid.NewGuid();
            var updateDto = new UpdateProductDto("Updated Name", "Updated Desc", 120, 15);
            var existingProduct = new ProductEntity("Old Name", "Old Desc", 100, 10);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(existingProduct);

       
            await _productService.UpdateAsync(productId, updateDto);

            
            _productRepositoryMock.Verify(repo => repo.Update(It.Is<ProductEntity>(p => p.Name == updateDto.Name)), Times.Once);
            _productRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenProductDoesNotExist_ShouldNotCallUpdate()
        {
            
            var productId = Guid.NewGuid();
            var updateDto = new UpdateProductDto("Updated Name", "Updated Desc", 120, 15);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((ProductEntity)null);

            
            await _productService.UpdateAsync(productId, updateDto);

            
            _productRepositoryMock.Verify(repo => repo.Update(It.IsAny<ProductEntity>()), Times.Never);
            _productRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

       

        [Fact]
        public async Task DeleteAsync_WhenProductExists_ShouldCallDeleteAndSaveChanges()
        {
            
            var productId = Guid.NewGuid();
            var existingProduct = new ProductEntity("To Be Deleted", "Desc", 50, 5);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(existingProduct);

          
            await _productService.DeleteAsync(productId);

           
            _productRepositoryMock.Verify(repo => repo.Delete(existingProduct), Times.Once);
            _productRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductDoesNotExist_ShouldNotCallDelete()
        {
            
            var productId = Guid.NewGuid();

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((ProductEntity)null);

            
            await _productService.DeleteAsync(productId);

           
            _productRepositoryMock.Verify(repo => repo.Delete(It.IsAny<ProductEntity>()), Times.Never);
            _productRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
    }
}