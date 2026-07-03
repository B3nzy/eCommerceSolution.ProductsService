namespace eCommerceSolution.ProductsService.Models.DTOs.HttpClient.Formats.InventoryMicroservice;

public class GetInventoryByProductIdResponse
{
    public Guid ProductId { get; set; }
    public int QuantityInStock { get; set; }
    public DateTime LastUpdated { get; set; }
}
