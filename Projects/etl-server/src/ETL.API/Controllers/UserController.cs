using ETL.API.Infrastructure;
using ETL.Application.Common.Constants;
using ETL.Application.User;
using ETL.Application.User.ChangeRole;
using ETL.Application.User.Create;
using ETL.Application.User.Delete;
using ETL.Application.User.Edit;
using ETL.Application.User.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETL.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetUserProfile(CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetUserProfileQuery(User), ct);
        return Ok(dto);
    }

    [Authorize(Policy = Policy.CanReadUser)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), ct);
        return this.FromResult(result);
    }

    [Authorize(Policy = Policy.CanReadAllUsers)]
    [HttpGet()]
    public async Task<IActionResult> GetAllUsers([FromQuery] int? first, [FromQuery] int? max, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllUsersQuery(first, max), ct);
        return this.FromResult(result);
    }

    [Authorize(Policy = Policy.CanCreateUser)]
    [HttpPost()]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        var createdId = result.Value;
        var location = Url.Action(null, "User", new { id = createdId }) ?? $"/api/user/{createdId}";

        return Created(location, new { id = createdId, message = $"User '{request.Username}' created." });
    }

    [Authorize(Policy = Policy.CanChangeUserRole)]
    [HttpPatch("change-role")]
    public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "User role updated." });
    }

    [Authorize]
    [HttpPut()]
    public async Task<IActionResult> EditUser([FromBody] EditUserCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "User updated." });
    }

    [Authorize(Policy = Policy.CanDeleteUser)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id), ct);
        return this.FromResult(result);
    }
}
