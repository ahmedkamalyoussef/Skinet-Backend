using Microsoft.AspNetCore.Mvc;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;
using Skinet.Core.Specifications;

namespace Skinet.API.Controllers;

public class ProductsController(IUnitOfWork _unitOfWork) : BaseApiController
{

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductSpecParams specParams)
    {
        var specification = new ProductFilterSortPaginationSpecification(specParams);
        return await CreatePagedResult(_unitOfWork.Repository<Product>(), specification, specParams.PageIndex, specParams.PageSize);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        _unitOfWork.Repository<Product>().Add(product);
        return await _unitOfWork.Complete() ?
            CreatedAtAction("GetProduct", new { id = product.Id }, product) : BadRequest("Product not created");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !_unitOfWork.Repository<Product>().Exists(id))
            return BadRequest("Product not found");
        _unitOfWork.Repository<Product>().Update(product);
        return await _unitOfWork.Complete() ? Ok("successfully updated") : BadRequest("Product not updated");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null)
            return NotFound();
        _unitOfWork.Repository<Product>().Delete(product);
        return await _unitOfWork.Complete() ? Ok("successfully deleted") : BadRequest("Product not deleted");
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetProductBrands()
    {
        var spec = new BrandListSpecification();
        return Ok(await _unitOfWork.Repository<Product>().ListAsync(spec));
    }
    [HttpGet("types")]
    public async Task<IActionResult> GetProductTypes()
    {
        var spec = new TypeListSpecification();
        return Ok(await _unitOfWork.Repository<Product>().ListAsync(spec));
    }
}