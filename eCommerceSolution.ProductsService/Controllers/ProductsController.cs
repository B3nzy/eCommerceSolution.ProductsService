using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceSolution.ProductsService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{

    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }   

    [HttpGet("get-all-products")]
    public IActionResult GetAllProducts()
    {
        return Ok(new { Message = "Products service is up and running!" });
    }



    [HttpGet("search/product-id/{productId}")]
    public async Task<IActionResult> GetProductById([FromRoute]Guid productId)
    {
        GetProductByIdResponse getProductByIdResponse =  await _mediator.Send(new GetProductByIdRequest() { ProductId = productId});
        return Ok(getProductByIdResponse);
    }

    //[HttpPost]
    //public async Task<IActionResult> CreateProduct([FromBody])
    //{
    //    return Ok(new { Message = "Create product endpoint is working!" });
    //}

}
