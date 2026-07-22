namespace eCommerceSolution.ProductsService.Models.DTOs.SearchProducts;

public class SearchIntent
{
    public string? SearchText { get; set; }
    public decimal? PriceLow { get; set; }
    public decimal? PriceHigh { get; set; }
}
