using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.HttpClients;
using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using MediatR;

namespace eCommerceSolution.ProductsService.Handlers;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, GetProductByIdResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<GetProductByIdHandler> _logger;
    private readonly InventoryMicroserviceHttpClient _inventoryMicroserviceHttpClient;

    public GetProductByIdHandler(ApplicationDbContext dbContext, ILogger<GetProductByIdHandler> logger, InventoryMicroserviceHttpClient inventoryMicroserviceHttpClient)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _inventoryMicroserviceHttpClient = inventoryMicroserviceHttpClient ?? throw new ArgumentNullException(nameof(inventoryMicroserviceHttpClient));
    }
    public async Task<GetProductByIdResponse?> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
        if (product == null)
        {
            return null;
        }
        var response = await _inventoryMicroserviceHttpClient.GetInventoryByProductId(product.ProductId);

        GetProductByIdResponse getProductByIdResponse = new GetProductByIdResponse()
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            Category = product.Category,
            Price = product.Price,
            QuantityInStock = response?.QuantityInStock ?? 0
        };

        return getProductByIdResponse;
    }
}
