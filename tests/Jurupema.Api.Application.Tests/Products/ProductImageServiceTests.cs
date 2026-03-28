using System.Linq.Expressions;
using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Services.Products;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Moq;
using Moq.AutoMock;

namespace Jurupema.Api.Application.Tests.Products;

public class ProductImageServiceTests
{
    private readonly AutoMocker _autoMocker;
    private readonly ProductImageService _sut;

    public ProductImageServiceTests()
    {
        _autoMocker = new AutoMocker();
        _sut = _autoMocker.CreateInstance<ProductImageService>();
    }

    [Fact]
    public async Task DeleteProductImageAsync_throws_ProductNotFoundException_when_product_missing()
    {
        // Arrange
        var product = new Product("missing", "missing", 1m);
        var image = new ProductImage(product.Id, "n/a.jpg");

        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync((Product?)null);

        // Act
        var act = async () =>
            await _sut.DeleteProductImageAsync(product.Id, image.Id, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<ProductNotFoundException>(act);
        Assert.Multiple(
            () => Assert.Equal(product.Id, ex.ProductId),
            () => _autoMocker.GetMock<IStorageClient>().Verify(
                s => s.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never));
    }

    [Fact]
    public async Task DeleteProductImageAsync_throws_ProductImageNotFoundException_when_image_not_on_product()
    {
        // Arrange
        var product = new Product("n", "d", 1m);
        var requestedImage = new ProductImage(product.Id, "not-persisted.jpg");

        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

        // Act
        var act = async () =>
            await _sut.DeleteProductImageAsync(product.Id, requestedImage.Id, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<ProductImageNotFoundException>(act);
        Assert.Multiple(
            () => Assert.Equal(product.Id, ex.ProductId),
            () => Assert.Equal(requestedImage.Id, ex.ProductImageId),
            () => _autoMocker.GetMock<IStorageClient>().Verify(
                s => s.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never));
    }

    [Fact]
    public async Task DeleteProductImageAsync_deletes_blob_removes_image_and_saves()
    {
        // Arrange
        var product = new Product("n", "d", 1m);
        var image = new ProductImage(product.Id, "photo.jpg");
        product.ProductImages.Add(image);

        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);
        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _autoMocker.GetMock<IStorageClient>()
            .Setup(s => s.DeleteFileAsync(image.Name, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteProductImageAsync(product.Id, image.Id, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.DoesNotContain(product.ProductImages, i => i.Id == image.Id),
            () => _autoMocker.GetMock<IStorageClient>().Verify(
                s => s.DeleteFileAsync(image.Name, It.IsAny<CancellationToken>()),
                Times.Once),
            () => _autoMocker.GetMock<IProductRepository>().Verify(r => r.SaveChangesAsync(), Times.Once));
    }
}
