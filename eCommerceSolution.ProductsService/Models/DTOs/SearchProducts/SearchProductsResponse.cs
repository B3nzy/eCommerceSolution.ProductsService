using eCommerceSolution.ProductsService.Models.Entities;

namespace eCommerceSolution.ProductsService.Models.DTOs.SearchProducts;

public class SearchProductsResponse
{
    public List<SearchResultDto>? products { get; set; }
}

// A simple DTO to hold your results
public class SearchResultDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public float SimilarityScore { get; set; } // Useful for debugging or sorting
}
