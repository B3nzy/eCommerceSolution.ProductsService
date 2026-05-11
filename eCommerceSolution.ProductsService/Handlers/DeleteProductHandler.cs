using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.DeleteProduct;
using eCommerceSolution.ProductsService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.ProductsService.Handlers;

public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, bool>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteProductHandler(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
    }

    public async Task<bool> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        // Approach 1: Fetch and Delete

        //Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
        //if(product == null)
        //{
        //    return false;
        //}
        //var deletedProduct = _dbContext.Products.Remove(product);
        //return true;


        // Approach 2: Direct Delete without Fetching

        int row = await _dbContext.Products
                .Where(p => p.ProductId == request.ProductId)
                .ExecuteDeleteAsync();

        if(row > 0)
        {
            return true;
        }
        return false;
    }
}
