using Jurupema.Api.Application.Storage;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;

namespace Jurupema.Api.Application.Products;

public class ProductService(IStorageClient storageClient, IProductRepository productRepository)
{
    public async Task UploadProductImageAsync(int productId, string fileName, Stream fileStream)
    {
        var product = await productRepository.GetByIdAsync(productId, p => p.ProductImages) ?? throw new Exception("Product not found");

        string url = "";
        var productImage = new ProductImage(product.Id, fileName, url);
        if (product.AlreadyHasImage(productImage))
            throw new Exception("Product already has this image");

        productRepository.InsertProductImage(productImage);
        await storageClient.UploadFileAsync(fileStream, fileName);
        await productRepository.SaveChangesAsync();
    }
}
