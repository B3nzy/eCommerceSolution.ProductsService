namespace eCommerceSolution.ProductsService.Models.Entities;

public class Product
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
}
