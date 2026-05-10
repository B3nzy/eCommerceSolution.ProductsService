using eCommerceSolution.ProductsService.Models.DTOs.CreateProduct;
using MediatR;

namespace eCommerceSolution.ProductsService.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductRequest, CreateProductResponse>
{
    public Task<CreateProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
