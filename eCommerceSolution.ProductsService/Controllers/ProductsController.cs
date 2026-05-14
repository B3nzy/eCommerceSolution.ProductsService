using eCommerceSolution.ProductsService.Models.DTOs.CreateProduct;
using eCommerceSolution.ProductsService.Models.DTOs.DeleteProduct;
using eCommerceSolution.ProductsService.Models.DTOs.GetProductById;
using eCommerceSolution.ProductsService.Models.DTOs.SearchProducts;
using eCommerceSolution.ProductsService.Models.DTOs.UpdateProduct;
using eCommerceSolution.ProductsService.Models.Entities;
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

    #region API Endpoints

    [HttpGet("get-all-products")]
    public async Task<IActionResult> GetAllProducts()
    {
        var sw = Stopwatch.StartNew();
        var getAllProductsResponse = await _mediator.Send(new Models.DTOs.GetAllProducts.GetAllProductsRequest());
        sw.Stop();
        _logger.LogInformation("[END] GetAllProducts request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        return Ok(getAllProductsResponse);
    }


    [HttpGet("search/product-id/{productId}")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid productId)
    {
        Stopwatch sw = Stopwatch.StartNew();
        GetProductByIdResponse getProductByIdResponse = await _mediator.Send(new GetProductByIdRequest() { ProductId = productId });
        sw.Stop();
        _logger.LogInformation("[END] GetProductById request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        if (getProductByIdResponse == null)
        {
            return NotFound(new { Message = $"Product with ID {productId} not found." });
        }
        return Ok(getProductByIdResponse);
    }


    [HttpPost("add-product")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest createProductRequest)
    {
        Stopwatch sw = Stopwatch.StartNew();
        CreateProductResponse createProductResponse = await _mediator.Send(createProductRequest);
        sw.Stop();
        _logger.LogInformation("[END] CreateProduct request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);

        return Ok(createProductResponse);
    }

    [HttpDelete("delete/product/{productId}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
    {
        Stopwatch sw = Stopwatch.StartNew();
        var deleteProductResponse = await _mediator.Send(new DeleteProductRequest() { ProductId = productId });
        sw.Stop();
        _logger.LogInformation("[END] DeleteProduct request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        if (!deleteProductResponse)
        {
            return NotFound(new { Message = $"Product with ID {productId} not found." });
        }
        return Ok(new { Success = true, Message = $"Product with ID {productId} has been deleted." });
    }

    [HttpPost("update-product")]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest updateProductRequest)
    {
        Stopwatch sw = Stopwatch.StartNew();
        var updateProductResponse = await _mediator.Send(updateProductRequest);
        sw.Stop();
        _logger.LogInformation("[END] UpdateProduct request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        if (!updateProductResponse)
        {
            return NotFound(new { Message = $"Product with ID {updateProductRequest.ProductId} not found." });
        }
        return Ok(new { Success = true, Message = $"Product with ID {updateProductRequest.ProductId} has been updated." });
    }

    [HttpGet("search/{searchString}")]
    public async Task<IActionResult> SearchProducts([FromRoute] string searchString)
    {
        Stopwatch sw = Stopwatch.StartNew();
        var searchProductsResponse = await _mediator.Send(new SearchProductsRequest() { SearchString = searchString });
        sw.Stop();
        _logger.LogInformation("[END] SearchProducts request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        return Ok(searchProductsResponse);
    }

    #endregion

}
