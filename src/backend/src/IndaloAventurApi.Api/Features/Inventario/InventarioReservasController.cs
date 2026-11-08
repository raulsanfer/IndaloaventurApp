using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CancelAdminReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CancelMiReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.ConfirmarDevolucionAdministracion;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.ConfirmarMiDevolucionMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CreateMiReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetAdminReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetAdminReservasMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetMiReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetMisReservasMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.RegistrarRetiradaReservaMaterial;
using IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.UpdateMiReservaMaterial;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591
namespace IndaloAventurApi.Api.Features.Inventario;

[ApiController]
[Route("api/inventario-reservas")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public sealed class InventarioReservasController(IMediator mediator) : ControllerBase
{
    [HttpGet("me")]
    [Authorize(Policy = AuthorizationPolicies.ClubMember)]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReservaMaterialDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ReservaMaterialDto>>> GetMyReservations(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetMisReservasMaterialQuery(User.GetRequiredUserId(), User.GetRequiredIsMember()), cancellationToken));

    [HttpGet("me/{reservaId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ClubMember)]
    [ProducesResponseType(typeof(ReservaMaterialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaMaterialDto>> GetMyReservation(Guid reservaId, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetMiReservaMaterialQuery(User.GetRequiredUserId(), User.GetRequiredIsMember(), reservaId), cancellationToken));

    [HttpPost("me")]
    [Authorize(Policy = AuthorizationPolicies.ClubMember)]
    [ProducesResponseType(typeof(ReservaMaterialDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReservaMaterialDto>> CreateMyReservation(
        [FromBody] ReservaMaterialRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMiReservaMaterialCommand(
            User.GetRequiredUserId(),
            User.GetRequiredIsMember(),
            request.InventarioMaterialId,
            request.CantidadSolicitada,
            request.FechaRetiradaPrevista,
            request.FechaDevolucionEstimada,
            request.Observaciones), cancellationToken);

        return CreatedAtAction(nameof(GetMyReservation), new { reservaId = result.Id }, result);
    }

    [HttpPut("me/{reservaId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ClubMember)]
    [ProducesResponseType(typeof(ReservaMaterialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReservaMaterialDto>> UpdateMyReservation(
        Guid reservaId,
        [FromBody] ReservaMaterialRequest request,
        CancellationToken cancellationToken)
        => Ok(await mediator.Send(new UpdateMiReservaMaterialCommand(
            User.GetRequiredUserId(),
            User.GetRequiredIsMember(),
            reservaId,
            request.InventarioMaterialId,
            request.CantidadSolicitada,
            request.FechaRetiradaPrevista,
            request.FechaDevolucionEstimada,
            request.Observaciones), cancellationToken));

    [HttpDelete("me/{reservaId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ClubMember)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelMyReservation(Guid reservaId, CancellationToken cancellationToken)
    {
        await mediator.Send(new CancelMiReservaMaterialCommand(User.GetRequiredUserId(), User.GetRequiredIsMember(), reservaId), cancellationToken);
        return NoContent();
    }

    [HttpPost("me/{reservaId:guid}/confirmacion-devolucion")]
    [Authorize(Policy = AuthorizationPolicies.ClubMember)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmMyReturn(Guid reservaId, CancellationToken cancellationToken)
    {
        await mediator.Send(new ConfirmarMiDevolucionMaterialCommand(User.GetRequiredUserId(), User.GetRequiredIsMember(), reservaId), cancellationToken);
        return NoContent();
    }

    [HttpGet("admin")]
    [Authorize(Policy = AuthorizationPolicies.InventoryManagement)]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReservaMaterialDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ReservaMaterialDto>>> GetAdminReservations(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetAdminReservasMaterialQuery(User.CanManageInventory()), cancellationToken));

    [HttpGet("admin/{reservaId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.InventoryManagement)]
    [ProducesResponseType(typeof(ReservaMaterialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaMaterialDto>> GetAdminReservation(Guid reservaId, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetAdminReservaMaterialQuery(User.CanManageInventory(), reservaId), cancellationToken));

    [HttpPost("admin/{reservaId:guid}/retirada")]
    [Authorize(Policy = AuthorizationPolicies.InventoryManagement)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterPickup(
        Guid reservaId,
        [FromBody] RegistrarRetiradaRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new RegistrarRetiradaReservaMaterialCommand(User.CanManageInventory(), reservaId, request.FechaRetirada), cancellationToken);
        return NoContent();
    }

    [HttpPost("admin/{reservaId:guid}/cancelacion")]
    [Authorize(Policy = AuthorizationPolicies.InventoryManagement)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelAdminReservation(
        Guid reservaId,
        [FromBody] CancelarReservaAdministracionRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new CancelAdminReservaMaterialCommand(User.CanManageInventory(), reservaId, request.ObservacionAdministracion), cancellationToken);
        return NoContent();
    }

    [HttpPost("admin/{reservaId:guid}/confirmacion-devolucion")]
    [Authorize(Policy = AuthorizationPolicies.InventoryManagement)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmAdminReturn(
        Guid reservaId,
        [FromBody] ConfirmarDevolucionAdministracionRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new ConfirmarDevolucionAdministracionCommand(
            User.CanManageInventory(),
            reservaId,
            request.FechaDevolucionEfectiva), cancellationToken);
        return NoContent();
    }

    public sealed record ReservaMaterialRequest(
        int InventarioMaterialId,
        int CantidadSolicitada,
        DateOnly FechaRetiradaPrevista,
        DateOnly FechaDevolucionEstimada,
        string? Observaciones);

    public sealed record RegistrarRetiradaRequest(DateOnly FechaRetirada);
    public sealed record CancelarReservaAdministracionRequest(string? ObservacionAdministracion);
    public sealed record ConfirmarDevolucionAdministracionRequest(DateOnly FechaDevolucionEfectiva);
}
#pragma warning restore CS1591
