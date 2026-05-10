using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;

namespace eCommerceSolution.ProductsService.Models.DTOs.GetAllProducts;

public class GetAllProductsResponse
{
    public List<GetProductByIdResponse>? ProductList { get; set; }
}
