using Microsoft.AspNetCore.Mvc;
using Skinet.API.DTOs;
using Skinet.Core.Entites;

namespace Skinet.API.Controllers;

public class BuggyController:BaseApiController
{
    [HttpGet("unauthorised")]
    public IActionResult GetUnauthorised()
    {
        return Unauthorized();
    }

    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("Bad Request");
    }

    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }

    [HttpGet("internalerror")]
    public IActionResult GetInternalError()
    {
        throw new Exception("Internal error");
    }

    [HttpPost("validationerror")]
    public IActionResult GetValidationError(CreateProductDto product)
    {
        return Ok();
    }
}