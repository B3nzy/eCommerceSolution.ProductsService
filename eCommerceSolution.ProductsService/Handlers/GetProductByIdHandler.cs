using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using MediatR;

namespace eCommerceSolution.ProductsService.Handlers;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, GetProductByIdResponse>
{
    public async Task<GetProductByIdResponse> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        return new GetProductByIdResponse()
        {
            ProductId = request.ProductId,
            ProductName = "Sample Product",
            Category = "Sample Category",
            Price = 99.99m,
            QuantityInStock = 100
        };
    }
}
