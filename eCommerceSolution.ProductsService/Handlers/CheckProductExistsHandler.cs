using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.CheckProductExists;
using MediatR;

namespace eCommerceSolution.ProductsService.Handlers;

public class CheckProductExistsHandler : IRequestHandler<CheckProductExistsRequest, bool>
{
    private readonly ApplicationDbContext _dbContext;

    public CheckProductExistsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(CheckProductExistsRequest request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(request.ProductId, cancellationToken);
        return product != null;
    }
}
