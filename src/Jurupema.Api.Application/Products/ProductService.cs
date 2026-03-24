using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Jurupema.Api.Domain.Specifications;

namespace Jurupema.Api.Application.Products;

public class ProductService(IStorageClient storageClient, IProductRepository productRepository)
{
    public async Task<PagedResult<ProductListItemResult>> QueryProductsAsync(
        QueryProductsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(parameters.PageIndex);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(parameters.PageSize, 0);

        var (items, totalCount) = await productRepository.GetPagedAsync(
            parameters.NameFilter,
            parameters.SortBy,
            parameters.SortDirection,
            parameters.PageIndex,
            parameters.PageSize,
            parameters.IncludeImages,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);
        var dtoItems = await Task.WhenAll(
            items.Select(p => MapToListItemAsync(p, parameters.IncludeImages, storageClient, cancellationToken)));

        return new PagedResult<ProductListItemResult>
        {
            Items = dtoItems.ToList(),
            PageIndex = parameters.PageIndex,
            PageSize = parameters.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<CreateProductResult> CreateProduct(CreateProductParameter parameter)
    {
        // Verify if a product with the same name already exists
        var existingProduct = await productRepository.GetAllAsync(new GetProductByNameSpecification(parameter.Name).GetExpression());
        if (existingProduct.Count > 0)
            throw new DuplicateProductNameException(parameter.Name);

        // Insert the register in the database
        var product = new Product(parameter.Name, parameter.Description, parameter.Price);
        productRepository.Insert(product);
        await productRepository.SaveChangesAsync();

        return new CreateProductResult(product);
    }

    public async Task UpdateProductAsync(UpdateProductParameter parameter)
    {
        var product = await productRepository.GetByIdAsync(parameter.Id)
            ?? throw new ProductNotFoundException(parameter.Id);

        product.Update(parameter.Name, parameter.Description, parameter.Price, parameter.Stock);
        productRepository.Update(product);
        await productRepository.SaveChangesAsync();
    }

    private static async Task<ProductListItemResult> MapToListItemAsync(
        Product product,
        bool includeImages,
        IStorageClient storageClient,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ProductImageListItemResult> images;
        if (!includeImages)
        {
            images = [];
        }
        else
        {
            var list = new List<ProductImageListItemResult>();
            foreach (var img in product.ProductImages)
            {
                var url = await storageClient.GetTemporaryReadUrlAsync(
                    img.Name,
                    TimeSpan.FromMinutes(10),
                    cancellationToken);
                list.Add(new ProductImageListItemResult(img, url));
            }

            images = list;
        }

        return ProductListItemResult.FromProduct(product, images);
    }
}
