using ETL.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETL.API.Controllers;
[Route("api/roles")]
[ApiController]
public class RoleController : ControllerBase
{
    [Authorize(Policy = Policy.CanReadRoles)]
    [HttpGet()]
    public IActionResult GetRoles()
    {
        return Ok(new { Roles = Role.GetAllRoles() });
    }
}
