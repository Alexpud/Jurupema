using System.Linq.Expressions;
using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Services;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Application.Tests.Builders;
using Jurupema.Api.Domain;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Moq;
using Moq.AutoMock;

namespace Jurupema.Api.Application.Tests.Products;

public class ProductServiceTests
{
    private readonly AutoMocker _autoMocker;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _autoMocker = new AutoMocker();
        _sut = _autoMocker.CreateInstance<ProductService>();
    }

    [Fact]
    public async Task QueryProductsAsync_when_include_images_and_storage_returns_url_sets_image_url()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        product.ProductImages.Add(new ProductImage(product.Id, "photo.jpg"));

        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetPagedAsync(
                null,
                ProductSortBy.Name,
                SortDirection.Ascending,
                0,
                20,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Product>)new List<Product> { product }, 1));

        var expectedUrl = "https://example.blob.core.windows.net/c/photo.jpg?sv=2021";
        _autoMocker.GetMock<IStorageClient>()
            .Setup(s => s.GetTemporaryReadUrlAsync(
                "photo.jpg",
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUrl);

        var parameters = new QueryProductsParameters(IncludeImages: true);

        // Act
        var page = await _sut.QueryProductsAsync(parameters, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.Single(page.Items),
            () => Assert.Single(page.Items[0].Images),
            () => Assert.Equal(expectedUrl, page.Items[0].Images[0].Url));
    }

    [Fact]
    public async Task QueryProductsAsync_when_include_images_and_storage_returns_null_sets_image_url_null()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        product.ProductImages.Add(new ProductImage(product.Id, "doc.png"));

        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetPagedAsync(
                null,
                ProductSortBy.Name,
                SortDirection.Ascending,
                0,
                20,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Product>)new List<Product> { product }, 1));

        _autoMocker.GetMock<IStorageClient>()
            .Setup(s => s.GetTemporaryReadUrlAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(string));

        var parameters = new QueryProductsParameters(IncludeImages: true);

        // Act
        var page = await _sut.QueryProductsAsync(parameters, CancellationToken.None);

        // Assert
        Assert.Null(page.Items[0].Images[0].Url);
    }

    [Fact]
    public async Task QueryProductsAsync_when_exclude_images_does_not_request_read_urls()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        product.ProductImages.Add(new ProductImage(product.Id, "x.png"));

        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetPagedAsync(
                null,
                ProductSortBy.Name,
                SortDirection.Ascending,
                0,
                20,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(([product], 1));

        var parameters = new QueryProductsParameters(IncludeImages: false);

        // Act
        var page = await _sut.QueryProductsAsync(parameters, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.Empty(page.Items[0].Images),
            () => _autoMocker.GetMock<IStorageClient>().Verify(
                s => s.GetTemporaryReadUrlAsync(
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()),
                Times.Never));
    }

    [Fact]
    public async Task UpdateProductAsync_throws_when_product_not_found()
    {
        // Arrange
        var missingProductId = Guid.NewGuid();
        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetByIdAsync(missingProductId, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync((Product?)null);

        var parameter = new UpdateProductParameterBuilder().WithId(missingProductId).Build();

        // Act
        var act = async () => await _sut.UpdateProductAsync(parameter);

        // Assert
        var ex = await Assert.ThrowsAsync<ProductNotFoundException>(act);
        Assert.Multiple(
            () => Assert.Equal(missingProductId, ex.ProductId),
            () => Assert.Contains(missingProductId.ToString(), ex.Message));
    }

    [Fact]
    public async Task UpdateProductAsync_updates_fields_and_persists_when_product_exists()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

        var parameter = new UpdateProductParameterBuilder().WithId(product.Id).Build();

        // Act
        await _sut.UpdateProductAsync(parameter);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(parameter.Name, product.Name),
            () => Assert.Equal(parameter.Description, product.Description),
            () => Assert.Equal(parameter.Price, product.Price),
            () => Assert.Equal(parameter.Stock, product.Stock),
            () => _autoMocker.GetMock<IProductRepository>().Verify(r => r.Update(product), Times.Once),
            () => _autoMocker.GetMock<IProductRepository>().Verify(r => r.SaveChangesAsync(), Times.Once));
    }
}
