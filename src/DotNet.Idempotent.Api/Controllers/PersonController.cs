using DotNet.Idempotent.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Idempotent.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok("Request OK");
    }

    [HttpPost]
    [ValidateIdempotent]
    public IActionResult Create()
    {
        return Ok("Request OK");
    }

    [HttpPut]
    [ValidateIdempotent]
    public IActionResult Update()
    {
        return Ok("Request OK");
    }
}