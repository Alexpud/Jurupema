using Bogus;
using Jurupema.Api.Application.Models;

namespace Jurupema.Api.Application.Tests.Builders;

public sealed class UpdateProductParameterBuilder
{
    private readonly Faker _faker;
    private int? _id;
    private string? _name;
    private string? _description;
    private decimal? _price;
    private int? _stock;

    public UpdateProductParameterBuilder(Faker? faker = null)
    {
        _faker = faker ?? new Faker();
    }

    public UpdateProductParameterBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public UpdateProductParameterBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UpdateProductParameterBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public UpdateProductParameterBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public UpdateProductParameterBuilder WithStock(int stock)
    {
        _stock = stock;
        return this;
    }

    public UpdateProductParameter Build() => new()
    {
        Id = _id ?? _faker.Random.Int(1, int.MaxValue),
        Name = _name ?? _faker.Commerce.ProductName(),
        Description = _description ?? _faker.Lorem.Sentence(),
        Price = _price ?? _faker.Random.Decimal(1, 1000),
        Stock = _stock ?? _faker.Random.Int(0, 500),
    };
}
