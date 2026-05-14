using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.SearchProducts;
using eCommerceSolution.ProductsService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.ProductsService.Handlers;

public class SearchProductsHandler : IRequestHandler<SearchProductsRequest, SearchProductsResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public SearchProductsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SearchProductsResponse> Handle(SearchProductsRequest request, CancellationToken cancellationToken)
    {
        List<Product>? productsFoundByName = await _dbContext.Products
                                                    .Where(p => p.ProductName.Contains(request.SearchString) || p.Category.Contains(request.SearchString))
                                                    .ToListAsync();

        return new SearchProductsResponse
        {
            products = productsFoundByName
        };
    }
}
