using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;

namespace Jurupema.Api.Application.Products;

public class ProductImageService(IStorageClient storageClient, IProductRepository productRepository)
{
    public async Task DeleteProductImageAsync(int productId, int productImageId, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(productId, p => p.ProductImages)
            ?? throw new ProductNotFoundException(productId);

        var image = product.ProductImages.FirstOrDefault(i => i.Id == productImageId)
            ?? throw new ProductImageNotFoundException(productId, productImageId);

        await storageClient.DeleteFileAsync(image.Name, cancellationToken);
        product.ProductImages.Remove(image);
        await productRepository.SaveChangesAsync();
    }

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
