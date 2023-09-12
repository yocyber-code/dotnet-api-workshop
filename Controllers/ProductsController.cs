using System.Runtime.CompilerServices;
using cmdev_dotnet_api.Data;
using cmdev_dotnet_api.DTOs.Product;
using cmdev_dotnet_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace cmdev_dotnet_api.Controllers;

[ApiController] //date annotation
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DatabaseContext databaseContext;

    public ProductsController(DatabaseContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    [HttpGet]
    public ActionResult<List<ProductResponse>> GetProduct()
    {
        List<ProductResponse> products = this.databaseContext.Products.Include(p => p.Category)
        .OrderByDescending(p => p.ProductId)
        .Select(ProductResponse.FromProduct).ToList();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult<ProductResponse> GetProductById(int id)
    {
        var products = databaseContext.Products.Include(p => p.Category)
        .SingleOrDefault(p => p.ProductId == id);
        if (products == null)
        {
            return NotFound();
        }
        return Ok(ProductResponse.FromProduct(products));
    }

    [HttpGet("search")]
    public ActionResult<List<ProductResponse>> GetProductByKeyword([FromQuery] string keyword = "")
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return Ok(new List<string>());
        }
        List<ProductResponse> result = databaseContext.Products.Include(p => p.Category)
        .Where(p => p.Name.ToLower().Contains(keyword.ToLower()))
        .Select(ProductResponse.FromProduct)
        .ToList();
        return Ok(result);
    }

    [HttpPost("")]
    public ActionResult<ProductModel> CreateProduct([FromForm] ProductRequest body)
    {
        Product product = body.Adapt<Product>();
        databaseContext.Products.Add(product);
        databaseContext.SaveChanges();
        return CreatedAtAction(nameof(CreateProduct), body);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateProducts(int id,[FromForm] ProductUpdateRequest body)
    {
        if (id <= 0 || id != body.ProductId) return NotFound();
        var products = databaseContext.Products.Find(id);
        if (products == null) return NotFound();
        Product product = body.Adapt(products);
        databaseContext.Products.Update(product);
        databaseContext.SaveChanges();
        return Ok(body);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteProductById(int id)
    {
        if (id <= 0) return NotFound();
        var products = databaseContext.Products.Find(id);
        if (products == null) return NotFound();
        databaseContext.Products.Remove(products);
        databaseContext.SaveChanges();
        return Ok();
    }


    public class ProductModel
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Price { get; set; } = 0;
    }

}
