using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using MediatR;

namespace eCommerceSolution.ProductsService.Handlers;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, GetProductByIdResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetProductByIdHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<GetProductByIdResponse?> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
        if (product == null)
        {
            return null;
        }
        return new GetProductByIdResponse()
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Category = product.Category,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock
        };
    }
}
