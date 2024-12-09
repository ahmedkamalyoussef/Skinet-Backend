using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skinet.API.RequestHelpers;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;
using Skinet.Core.Specifications;
using Skinet.Infrastructure.Data;

namespace Skinet.API.Controllers;
[ApiController]
[Route("[controller]")]
public class ProductsController(IGenericRepository<Product> _repository): ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
    {
        var specification=new ProductFilterSortPaginationSpecification(specParams);
        var products = await _repository.ListAsync(specification); 
        var count = await _repository.CountAsync(specification);
        var pagination =new Pagination<Product>(specParams.PageIndex, specParams.PageSize, count,products);
        return Ok(pagination);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        _repository.Add(product);
        return await _repository.SaveAsync() ? 
            CreatedAtAction("GetProduct",new{id=product.Id},product) : BadRequest("Product not created");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id||!_repository.Exists(id))
            return BadRequest("Product not found");
        _repository.Update(product);
        return await _repository.SaveAsync() ? Ok("successfully updated") : BadRequest("Product not updated");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if(product == null)
            return NotFound();
        _repository.Delete(product);
        return await _repository.SaveAsync() ? Ok("successfully deleted") : BadRequest("Product not deleted");
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetProductBrands()
    {
        var spec = new BrandListSpecification();
        return Ok(await _repository.ListAsync(spec));
    }
    [HttpGet("types")]
    public async Task<IActionResult> GetProductTypes()
    {
        var spec = new TypeListSpecification();
        return Ok(await _repository.ListAsync(spec));
    }
}