using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.GetAllProducts;
using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using eCommerceSolution.ProductsService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.ProductsService.Handlers;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsRequest, GetAllProductsResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetAllProductsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetAllProductsResponse> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
    {
        List<Product> products = await _dbContext.Products.ToListAsync();
        List<GetProductByIdResponse> getProductByIdResponse = products
                                                        .Select(p => new GetProductByIdResponse()
                                                        {
                                                            Category = p.Category,
                                                            Price = p.Price,
                                                            ProductId = p.ProductId,
                                                            ProductName = p.ProductName,
                                                            QuantityInStock = p.QuantityInStock
                                                        })
                                                        .ToList();

        GetAllProductsResponse getAllProductsResponse = new GetAllProductsResponse() { ProductList = getProductByIdResponse };
        return getAllProductsResponse;
    }
}
