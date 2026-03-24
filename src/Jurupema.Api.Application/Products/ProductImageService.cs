using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;

namespace Jurupema.Api.Application.Products;

public class ProductImageService(IStorageClient storageClient, IProductRepository productRepository)
{
    public async Task UploadProductImageAsync(int productId, string fileName, Stream fileStream)
    {
        var product = await productRepository.GetByIdAsync(productId, p => p.ProductImages)
            ?? throw new ProductNotFoundException(productId);

        if (!product.TryAddImage(fileName))
            throw new DuplicateProductImageException(fileName);

        await storageClient.UploadFileAsync(fileStream, fileName);
        await productRepository.SaveChangesAsync();
    }
}
