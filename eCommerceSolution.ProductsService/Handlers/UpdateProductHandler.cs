using eCommerce.Microservices.Events.Product;
using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.UpdateProduct;
using eCommerceSolution.ProductsService.Models.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.ProductsService.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, bool>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateProductHandler(ApplicationDbContext dbContext, ILogger<UpdateProductHandler> logger, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<bool> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        // Find the existing product by ID then update its properties with the new values from the request
        Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
        if (product == null)
        {
            return false;
        }
        product.ProductName = request.ProductName;
        product.Category = request.Category;
        product.Price = request.Price;
        int rows = await _dbContext.SaveChangesAsync();


        // Alternative approach using Attach and setting the state to Modified
        //_dbContext.Products.Update(new Product
        //{
        //    ProductId = request.ProductId,
        //    ProductName = request.ProductName,
        //    Category = request.Category,
        //    Price = request.Price
        //});
        //await _dbContext.SaveChangesAsync(cancellationToken);

        ProductStock productStockEvent = new ProductStock()
        {
            ProductId = request.ProductId,
            QuantityInStock = request.QuantityInStock,
        };

        await _publishEndpoint.Publish(productStockEvent, cancellationToken);

        return true;
    }
}
