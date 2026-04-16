using Hris.Demo.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hris.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BrandingController(IOptions<BrandingOptions> options) : ControllerBase
{
    [HttpGet]
    public ActionResult<BrandingOptions> Get() => Ok(options.Value);
}
