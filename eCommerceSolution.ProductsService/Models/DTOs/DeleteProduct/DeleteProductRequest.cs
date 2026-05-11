using MediatR;

namespace eCommerceSolution.ProductsService.Models.DTOs.DeleteProduct;

public class DeleteProductRequest:IRequest<bool>
{
    public Guid ProductId { get; set; }
}
