using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DailyCost.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(id, out var userId)) return userId;
        throw new InvalidOperationException("Invalid user id claim");
    }
}

