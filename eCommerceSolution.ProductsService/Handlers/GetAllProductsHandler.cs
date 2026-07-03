using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.HttpClients;
using eCommerceSolution.ProductsService.Models.DTOs.GetAllProducts;
using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using eCommerceSolution.ProductsService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.ProductsService.Handlers;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsRequest, GetAllProductsResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly InventoryMicroserviceHttpClient _inventoryMicroserviceHttpClient;
    private readonly ILogger<GetAllProductsHandler> _logger;

    public GetAllProductsHandler(ApplicationDbContext dbContext, InventoryMicroserviceHttpClient inventoryMicroserviceHttpClient, ILogger<GetAllProductsHandler> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _inventoryMicroserviceHttpClient = inventoryMicroserviceHttpClient ?? throw new ArgumentNullException(nameof(inventoryMicroserviceHttpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                                                            ProductName = p.ProductName
                                                        })
                                                        .ToList();

        foreach (var product in getProductByIdResponse)
        {
            var inventoryResponse = await _inventoryMicroserviceHttpClient.GetInventoryByProductId(product.ProductId);
            if (inventoryResponse == null)
            {
                _logger.LogInformation($"Failed to get inventory for product {product.ProductId}. Setting QuantityInStock to 0");
                product.QuantityInStock = 0;
            }
            else
            {
                product.QuantityInStock = inventoryResponse.QuantityInStock;
            }
        }

        GetAllProductsResponse getAllProductsResponse = new GetAllProductsResponse() { ProductList = getProductByIdResponse };
        return getAllProductsResponse;
    }
}
