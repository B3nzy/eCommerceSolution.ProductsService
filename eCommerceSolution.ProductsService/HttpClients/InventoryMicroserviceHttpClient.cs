using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using eCommerceSolution.ProductsService.Models.DTOs.HttpClient.Formats.InventoryMicroservice;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace eCommerceSolution.ProductsService.HttpClients;

public class InventoryMicroserviceHttpClient
{
    private readonly HttpClient _httpClient;
    public readonly ILogger<InventoryMicroserviceHttpClient> _logger;

    public InventoryMicroserviceHttpClient(HttpClient httpClient, ILogger<InventoryMicroserviceHttpClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GetInventoryByProductIdResponse> GetInventoryByProductId(Guid productId)
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/InventoryManagement/get-inventory-by-product-id/{productId}");
        if (httpResponse.IsSuccessStatusCode)
        {
            GetInventoryByProductIdResponse? response = await httpResponse.Content.ReadFromJsonAsync<GetInventoryByProductIdResponse>();
            if (response != null)
            {
                return response;
            }
        }
        return null;
    }
}
