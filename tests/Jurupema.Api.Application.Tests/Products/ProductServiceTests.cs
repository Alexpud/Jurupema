using System.Linq.Expressions;
using Bogus;
using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Products;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Application.Tests.Builders;
using Jurupema.Api.Domain;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Moq;

namespace Jurupema.Api.Application.Tests.Products;

public class ProductServiceTests
{
    private const int TestDataSeed = 2_718_281;

    [Fact]
    public async Task QueryProductsAsync_when_include_images_and_storage_returns_url_sets_image_url()
    {
        // Arrange
        var faker = new Faker { Random = new Randomizer(TestDataSeed) };
        var product = new ProductBuilder(faker).Build();
        SetEntityId(product, 7);
        product.ProductImages.Add(new ProductImage(product.Id, "photo.jpg"));

        var repo = new Mock<IProductRepository>();
        repo
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
        var storage = new Mock<IStorageClient>();
        storage
            .Setup(s => s.GetTemporaryReadUrlAsync(
                "photo.jpg",
                TimeSpan.FromMinutes(1),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUrl);

        var sut = new ProductService(storage.Object, repo.Object);
        var parameters = new QueryProductsParameters(IncludeImages: true);

        // Act
        var page = await sut.QueryProductsAsync(parameters, CancellationToken.None);

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
        var faker = new Faker { Random = new Randomizer(TestDataSeed) };
        var product = new ProductBuilder(faker).Build();
        SetEntityId(product, 8);
        product.ProductImages.Add(new ProductImage(product.Id, "doc.png"));

        var repo = new Mock<IProductRepository>();
        repo
            .Setup(r => r.GetPagedAsync(
                null,
                ProductSortBy.Name,
                SortDirection.Ascending,
                0,
                20,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Product>)new List<Product> { product }, 1));

        var storage = new Mock<IStorageClient>();
        storage
            .Setup(s => s.GetTemporaryReadUrlAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(string));

        var sut = new ProductService(storage.Object, repo.Object);
        var parameters = new QueryProductsParameters(IncludeImages: true);

        // Act
        var page = await sut.QueryProductsAsync(parameters, CancellationToken.None);

        // Assert
        Assert.Null(page.Items[0].Images[0].Url);
    }

    [Fact]
    public async Task QueryProductsAsync_when_exclude_images_does_not_request_read_urls()
    {
        // Arrange
        var faker = new Faker { Random = new Randomizer(TestDataSeed) };
        var product = new ProductBuilder(faker).Build();
        SetEntityId(product, 9);
        product.ProductImages.Add(new ProductImage(product.Id, "x.png"));

        var repo = new Mock<IProductRepository>();
        repo
            .Setup(r => r.GetPagedAsync(
                null,
                ProductSortBy.Name,
                SortDirection.Ascending,
                0,
                20,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Product>)new List<Product> { product }, 1));

        var storage = new Mock<IStorageClient>();
        var sut = new ProductService(storage.Object, repo.Object);
        var parameters = new QueryProductsParameters(IncludeImages: false);

        // Act
        var page = await sut.QueryProductsAsync(parameters, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.Empty(page.Items[0].Images),
            () => storage.Verify(
                s => s.GetTemporaryReadUrlAsync(
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()),
                Times.Never));
    }

    private static void SetEntityId(Product product, int id)
    {
        var idProp = product.GetType().BaseType!.GetProperty("Id")!;
        idProp.GetSetMethod(true)!.Invoke(product, [id]);
    }

    [Fact]
    public async Task UpdateProductAsync_throws_when_product_not_found()
    {
        // Arrange
        var faker = new Faker { Random = new Randomizer(TestDataSeed) };
        var missingProductId = faker.Random.Int(500_000, int.MaxValue);
        var repo = new Mock<IProductRepository>();
        repo
            .Setup(r => r.GetByIdAsync(missingProductId, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync((Product?)null);

        var storage = new Mock<IStorageClient>();
        var sut = new ProductService(storage.Object, repo.Object);
        var parameter = new UpdateProductParameterBuilder(faker).WithId(missingProductId).Build();

        // Act
        var act = async () => await sut.UpdateProductAsync(parameter);

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
        var faker = new Faker { Random = new Randomizer(TestDataSeed) };
        var productId = faker.Random.Int(1, 100_000);
        var repo = new Mock<IProductRepository>();
        var product = new ProductBuilder(faker).Build();
        repo
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

        var storage = new Mock<IStorageClient>();
        var sut = new ProductService(storage.Object, repo.Object);
        var parameter = new UpdateProductParameterBuilder(faker).WithId(productId).Build();

        // Act
        await sut.UpdateProductAsync(parameter);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(parameter.Name, product.Name),
            () => Assert.Equal(parameter.Description, product.Description),
            () => Assert.Equal(parameter.Price, product.Price),
            () => Assert.Equal(parameter.Stock, product.Stock),
            () => repo.Verify(r => r.Update(product), Times.Once),
            () => repo.Verify(r => r.SaveChangesAsync(), Times.Once));
    }
}
