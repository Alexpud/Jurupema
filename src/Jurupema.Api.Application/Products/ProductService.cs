using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Jurupema.Api.Domain.Specifications;

namespace Jurupema.Api.Application.Products;

public class ProductService(IStorageClient storageClient, IProductRepository productRepository)
{
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

    public async Task UploadProductImageAsync(int productId, string fileName, Stream fileStream)
    {
        var product = await productRepository.GetByIdAsync(productId, p => p.ProductImages)
            ?? throw new ProductNotFoundException(productId);

        string url = "";
        var productImage = new ProductImage(product.Id, fileName, url);
        if (product.AlreadyHasImage(productImage))
            throw new DuplicateProductImageException(fileName);

        
        await storageClient.UploadFileAsync(fileStream, fileName);
        await productRepository.SaveChangesAsync();
    }
}
