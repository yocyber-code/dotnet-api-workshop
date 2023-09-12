using System.Runtime.CompilerServices;
using cmdev_dotnet_api.Data;
using cmdev_dotnet_api.DTOs.Product;
using cmdev_dotnet_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mapster;
using cmdev_dotnet_api.services;
using cmdev_dotnet_api.interfaces;

namespace cmdev_dotnet_api.Controllers;

[ApiController] //date annotation
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DatabaseContext databaseContext;
    private readonly IProductService productService;

    public ProductsController(DatabaseContext databaseContext, IProductService productService)
    {
        this.databaseContext = databaseContext;
        this.productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProduct()
    {
        List<ProductResponse> products = (await productService.FindAll()).Select(ProductResponse.FromProduct).ToList();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(int id)
    {
        var product = await productService.FindById(id);
        return Ok(product.Adapt<ProductResponse>());
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProductResponse>>> GetProductByKeyword([FromQuery] string keyword = "")
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return Ok(new List<string>());
        }
        List<ProductResponse> result = (await productService.Search(keyword)).Select(ProductResponse.FromProduct).ToList();
        return Ok(result);
    }

    [HttpPost("")]
    public async Task<ActionResult<ProductModel>> CreateProduct([FromForm] ProductRequest body)
    {
        Product product = body.Adapt<Product>();
        await productService.Create(product);
        return CreatedAtAction(nameof(CreateProduct), body);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductUpdateRequest>> UpdateProducts(int id, [FromForm] ProductUpdateRequest body)
    {
        if (id <= 0 || id != body.ProductId) return BadRequest();
        var product = await productService.FindById(id);
        if (product == null) return NotFound();
        Product new_product = body.Adapt(product);
        await productService.Update(new_product);
        return Ok(body);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductById(int id)
    {
        if (id <= 0) return NotFound();
        var products = await productService.FindById(id);
        if (products == null) return NotFound();
        await productService.Delete(products);
        return Ok();
    }

    public class ProductModel
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Price { get; set; } = 0;
    }

}
