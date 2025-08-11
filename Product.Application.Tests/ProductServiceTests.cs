using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Moq;
using Product.Application.DTOs;
using Product.Application.Services;
using Product.Domain.Interfaces;
using ProductEntity = Product.Domain.Models.Product;

namespace Product.Application.Tests
{
    public class ProductServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
            _mapperMock = _fixture.Freeze<Mock<IMapper>>();
            _productService = _fixture.Create<ProductService>();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedDto_WhenProductExists()
        {
            var product = _fixture.Create<ProductEntity>();
            var productDto = _fixture.Create<ProductDto>();

            _productRepositoryMock.Setup(r => r.GetByIdAsync(product.Id))
                .ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                .Returns(productDto);

            var result = await _productService.GetByIdAsync(product.Id);

            result.Should().BeEquivalentTo(productDto);
            _productRepositoryMock.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
            _mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();
            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync((ProductEntity)null);

            var result = await _productService.GetByIdAsync(productId);

            result.Should().BeNull();
            _mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfProductDtos_WhenProductsExist()
        {
            var products = _fixture.CreateMany<ProductEntity>(3).ToList();
            var productDtos = _fixture.CreateMany<ProductDto>(3).ToList();

            _productRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

            var result = await _productService.GetAllAsync();

            result.Should().BeEquivalentTo(productDtos);
            _mapperMock.Verify(m => m.Map<IEnumerable<ProductDto>>(products), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges_AndReturnDto()
        {
            var createDto = _fixture.Create<CreateProductDto>();
            var expectedDto = _fixture.Create<ProductDto>();

            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<ProductEntity>()))
                .Returns(expectedDto);

            var result = await _productService.CreateAsync(createDto);

            result.Should().BeEquivalentTo(expectedDto);
            _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ProductEntity>()), Times.Once);
            _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<ProductEntity>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndSave_WhenProductExists()
        {
            var productId = Guid.NewGuid();
            var updateDto = _fixture.Create<UpdateProductDto>();
            var existingProduct = _fixture.Create<ProductEntity>();

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(existingProduct);

            await _productService.UpdateAsync(productId, updateDto);

            _productRepositoryMock.Verify(r => r.Update(It.IsAny<ProductEntity>()), Times.Once);
            _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotCallUpdate_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();
            var updateDto = _fixture.Create<UpdateProductDto>();

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((ProductEntity)null);

            await _productService.UpdateAsync(productId, updateDto);

            _productRepositoryMock.Verify(r => r.Update(It.IsAny<ProductEntity>()), Times.Never);
            _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAndSave_WhenProductExists()
        {
            var existingProduct = _fixture.Create<ProductEntity>();

            _productRepositoryMock.Setup(r => r.GetByIdAsync(existingProduct.Id))
                                  .ReturnsAsync(existingProduct);

            await _productService.DeleteAsync(existingProduct.Id);


            _productRepositoryMock.Verify(r => r.Delete(existingProduct), Times.Once);
            _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotCallDelete_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync((ProductEntity)null);

            await _productService.DeleteAsync(productId);

            _productRepositoryMock.Verify(r => r.Delete(It.IsAny<ProductEntity>()), Times.Never);
            _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenRepositoryFails()
        {
            var id = Guid.NewGuid();
            _productRepositoryMock.Setup(r => r.GetByIdAsync(id))
                .ThrowsAsync(new Exception("DB error"));

            Func<Task> act = () => _productService.GetByIdAsync(id);

            await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
        }
    }
}
