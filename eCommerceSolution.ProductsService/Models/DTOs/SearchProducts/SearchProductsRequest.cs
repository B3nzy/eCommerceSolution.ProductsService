using MediatR;

namespace eCommerceSolution.ProductsService.Models.DTOs.SearchProducts;

public class SearchProductsRequest : IRequest<SearchProductsResponse>
{
    public string? SearchString { get; set; }
}
