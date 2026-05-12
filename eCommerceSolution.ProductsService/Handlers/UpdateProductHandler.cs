using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.UpdateProduct;
using eCommerceSolution.ProductsService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.ProductsService.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, bool>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateProductHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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
        product.QuantityInStock = request.QuantityInStock;
        int rows = await _dbContext.SaveChangesAsync();
        return true;

        // Alternative approach using Attach and setting the state to Modified
        //_dbContext.Products.Update(new Product
        //{
        //    ProductId = request.ProductId,
        //    ProductName = request.ProductName,
        //    Category = request.Category,
        //    Price = request.Price,
        //    QuantityInStock = request.QuantityInStock
        //});
        //await _dbContext.SaveChangesAsync(cancellationToken);
        //return true;
    }
}
