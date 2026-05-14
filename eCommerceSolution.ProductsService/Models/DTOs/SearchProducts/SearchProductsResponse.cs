using eCommerceSolution.ProductsService.Models.Entities;

namespace eCommerceSolution.ProductsService.Models.DTOs.SearchProducts;

public class SearchProductsResponse
{
    public List<Product>? products { get; set; }
}
