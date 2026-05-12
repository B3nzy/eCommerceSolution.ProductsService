using MediatR;

namespace eCommerceSolution.ProductsService.Models.DTOs.UpdateProduct;

public class UpdateProductRequest:IRequest<bool>
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
}
