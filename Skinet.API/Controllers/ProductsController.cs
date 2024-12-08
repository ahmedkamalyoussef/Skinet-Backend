using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;
using Skinet.Infrastructure.Data;

namespace Skinet.API.Controllers;
[ApiController]
[Route("[controller]")]
public class ProductsController(IProductRepository _productRepository): ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
        var products = await _productRepository.GetProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        _productRepository.AddProduct(product);
        return await _productRepository.SaveChangesAsync() ? 
            CreatedAtAction("GetProduct",new{id=product.Id},product) : BadRequest("Product not created");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id||!_productRepository.ProductExists(id))
            return BadRequest("Product not found");
        _productRepository.UpdateProduct(product);
        return await _productRepository.SaveChangesAsync() ? Ok("successfully updated") : BadRequest("Product not updated");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if(product == null)
            return NotFound();
        _productRepository.DeleteProduct(product);
        return await _productRepository.SaveChangesAsync() ? Ok("successfully deleted") : BadRequest("Product not deleted");
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetProductBrands()
    {
        return Ok(await _productRepository.GetBrandsAsync());
    }
    [HttpGet("types")]
    public async Task<IActionResult> GetProductTypes()
    {
        return Ok(await _productRepository.GetTypesAsync());
    }
}