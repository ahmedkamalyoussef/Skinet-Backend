using Microsoft.AspNetCore.Mvc;
using Skinet.API.RequestHelpers;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;

namespace Skinet.API.Controllers;
[ApiController]
[Route("[controller]")]
public class BaseApiController : ControllerBase
{
    protected async Task<IActionResult> CreatePagedResult<T>(IGenericRepository<T> repository,
        ISpecification<T> specification, int pageIndex, int pageSize) where T : BaseEntity
    {
        var items = await repository.ListAsync(specification);
        var count = await repository.CountAsync(specification);
        var pagination = new Pagination<T>(pageIndex, pageSize, count, items);
        return Ok(pagination);
    }

    protected async Task<IActionResult> CreatePagedResult<T, TDto>(IGenericRepository<T> repository,
        ISpecification<T> specification, int pageIndex, int pageSize, Func<T, TDto> toDto) where T : BaseEntity, IDtoConvertable
    {
        var items = await repository.ListAsync(specification);
        var count = await repository.CountAsync(specification);
        var dtoItems = items.Select(toDto).ToList();
        var pagination = new Pagination<TDto>(pageIndex, pageSize, count, dtoItems);
        return Ok(pagination);
    }
}