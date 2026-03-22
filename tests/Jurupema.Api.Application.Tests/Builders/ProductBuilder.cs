using Bogus;
using Jurupema.Api.Domain.Entities;

namespace Jurupema.Api.Application.Tests.Builders;

public sealed class ProductBuilder
{
    private readonly Faker _faker;
    private string? _name;
    private string? _description;
    private decimal? _price;

    public ProductBuilder(Faker? faker = null)
    {
        _faker = faker ?? new Faker();
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public Product Build()
    {
        var name = _name ?? _faker.Commerce.ProductName();
        var description = _description ?? _faker.Commerce.ProductDescription();
        var price = _price ?? _faker.Random.Decimal(1, 500);
        return new Product(name, description, price);
    }
}
