using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.CreateProduct;
using eCommerceSolution.ProductsService.Models.Entities;
using MediatR;
using eCommerce.Microservices.Events.Product;
using MassTransit;

namespace eCommerceSolution.ProductsService.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductRequest, CreateProductResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateProductHandler(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<CreateProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Product product = new Product()
        {
            ProductId = Guid.NewGuid(),
            ProductName = request.ProductName,
            ProductDescription = request.ProductDescription,
            Category = request.Category,
            Price = request.Price
        };

        ProductStock productStock = new ProductStock()
        {
            ProductId = product.ProductId,
            QuantityInStock = request.QuantityInStock
        };

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        await _publishEndpoint.Publish(productStock, cancellationToken);

        return new CreateProductResponse()
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            Category = product.Category,
            Price = product.Price,
            QuantityInStock = productStock.QuantityInStock
        };
    }
}
