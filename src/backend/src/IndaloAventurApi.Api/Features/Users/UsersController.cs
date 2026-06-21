using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Features.Users.CreateManagedUser;
using IndaloAventurApi.Application.Features.Users.DeactivateManagedUser;
using IndaloAventurApi.Application.Features.Users.ListManagedUsers;
using IndaloAventurApi.Application.Features.Users.ReactivateManagedUser;
using IndaloAventurApi.Application.Features.Users.UpdateManagedUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndaloAventurApi.Api.Features.Users;

[ApiController]
[Route("api/users")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ManagedUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ManagedUserDto>>> List([FromQuery] string? email, CancellationToken cancellationToken)
    {
        var users = await mediator.Send(new ListManagedUsersQuery(email), cancellationToken);
        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateManagedUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(List), new { id = userId }, userId);
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateManagedUserRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateManagedUserCommand(userId, request.Email, request.IsMember, request.Roles), cancellationToken);
        return NoContent();
    }

    [HttpPost("{userId:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deactivate(Guid userId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeactivateManagedUserCommand(userId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{userId:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reactivate(Guid userId, CancellationToken cancellationToken)
    {
        await mediator.Send(new ReactivateManagedUserCommand(userId), cancellationToken);
        return NoContent();
    }

    public sealed record UpdateManagedUserRequest(string Email, bool IsMember, IReadOnlyCollection<string> Roles);
}
