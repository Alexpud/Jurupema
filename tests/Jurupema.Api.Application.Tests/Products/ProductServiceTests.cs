using System.Linq.Expressions;
using Bogus;
using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Products;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Application.Tests.Builders;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Moq;

namespace Jurupema.Api.Application.Tests.Products;

public class ProductServiceTests
{
    private const int TestDataSeed = 2_718_281;

    [Fact]
    public async Task UpdateProductAsync_throws_when_product_not_found()
    {
        // Arrange
        var faker = new Faker { Random = new Randomizer(TestDataSeed) };
        var missingProductId = faker.Random.Int(500_000, int.MaxValue);
        var storage = new Mock<IStorageClient>();
        var repo = new Mock<IProductRepository>();
        repo
            .Setup(r => r.GetByIdAsync(missingProductId, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync((Product?)null);

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
        var storage = new Mock<IStorageClient>();
        var repo = new Mock<IProductRepository>();
        var product = new ProductBuilder(faker).Build();
        repo
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<Expression<Func<Product, object>>>()))
            .ReturnsAsync(product);

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
