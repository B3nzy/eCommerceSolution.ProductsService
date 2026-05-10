using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.CreateProduct;
using eCommerceSolution.ProductsService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.ProductsService.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductRequest, CreateProductResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateProductHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Product product = new Product()
        {
            ProductId = Guid.NewGuid(),
            ProductName = request.ProductName,
            Category = request.Category,
            Price = request.Price,
            QuantityInStock = request.QuantityInStock
        };

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        return new CreateProductResponse()
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Category = product.Category,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock
        };
    }
}
