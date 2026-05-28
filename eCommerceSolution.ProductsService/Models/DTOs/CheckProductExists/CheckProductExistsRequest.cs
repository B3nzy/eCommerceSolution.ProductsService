using MediatR;

namespace eCommerceSolution.ProductsService.Models.DTOs.CheckProductExists;

public class CheckProductExistsRequest : IRequest<bool>
{
    public Guid ProductId { get; set; }
}
