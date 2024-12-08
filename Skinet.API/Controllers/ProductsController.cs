using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skinet.Core.Entites;
using Skinet.Infrastructure.Data;

namespace Skinet.API.Controllers;
[ApiController]
[Route("[controller]")]
public class ProductsController(StoreContext context): ControllerBase
{
    private readonly StoreContext _context = context;
    
    
    
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        _context.Products.Add(product);
        int affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0 ? Ok(product) : BadRequest();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id||!ProductExists(id))
            return BadRequest("Product not found");
        _context.Entry(product).State = EntityState.Modified;
        return await _context.SaveChangesAsync()>0 ? Ok(product) : BadRequest("Product not updated");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if(product == null)
            return NotFound();
        _context.Products.Remove(product);
        return await _context.SaveChangesAsync() > 0 ? Ok(product) : BadRequest("Product not deleted");
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}