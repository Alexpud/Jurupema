using Jurupema.Api.Domain.Entities;

namespace Jurupema.Api.Application.Models;

public record CreateProductResult
{
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal Price { get; }
    public DateTime CreatedAt { get; }

    public CreateProductResult(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        Price = product.Price;
        CreatedAt = product.CreatedAt;
    }
}
