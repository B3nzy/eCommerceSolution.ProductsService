using MediatR;

namespace eCommerceSolution.ProductsService.Models.DTOs.CreateProduct;

public class CreateProductRequest:IRequest<CreateProductResponse>
{
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
}
