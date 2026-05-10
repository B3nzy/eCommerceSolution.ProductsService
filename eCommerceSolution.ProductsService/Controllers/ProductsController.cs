using eCommerceSolution.ProductsService.Models.DTOs.CreateProduct;
using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace eCommerceSolution.ProductsService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{

    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("get-all-products")]
    public IActionResult GetAllProducts()
    {
        return Ok(new { Message = "Products service is up and running!" });
    }



    [HttpGet("search/product-id/{productId}")]
    public async Task<IActionResult> GetProductById([FromRoute]Guid productId)
    {
        Stopwatch sw = Stopwatch.StartNew();
        GetProductByIdResponse getProductByIdResponse =  await _mediator.Send(new GetProductByIdRequest() { ProductId = productId});
        sw.Stop();
        _logger.LogInformation("[END] GetProductById request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        if(getProductByIdResponse == null)
        {
            return NotFound(new { Message = $"Product with ID {productId} not found." });
        }
        return Ok(getProductByIdResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest createProductRequest)
    {
        Stopwatch sw = Stopwatch.StartNew();
        CreateProductResponse createProductResponse = await _mediator.Send(createProductRequest);
        sw.Stop();
        _logger.LogInformation("[END] CreateProduct request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);

        return Ok(createProductResponse);
    }

}
