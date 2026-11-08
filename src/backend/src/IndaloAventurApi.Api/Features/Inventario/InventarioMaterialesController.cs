using IndaloAventurApi.Api.Security;using IndaloAventurApi.Application.Abstractions.Inventario;using IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.CreateInventarioMaterial;using IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.DeleteInventarioMaterial;using IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.GetAllInventarioMateriales;using IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.GetInventarioMaterialById;using IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.UpdateInventarioMaterial;using MediatR;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
#pragma warning disable CS1591
namespace IndaloAventurApi.Api.Features.Inventario;
[ApiController][Route("api/inventario-materiales")][Authorize(Policy=AuthorizationPolicies.Authenticated)]
public sealed class InventarioMaterialesController(IMediator mediator):ControllerBase
{
 [HttpGet] public async Task<ActionResult<IReadOnlyCollection<InventarioMaterialDto>>> GetAll(CancellationToken ct)=>Ok(await mediator.Send(new GetAllInventarioMaterialesQuery(),ct));
 [HttpGet("{id:int}")] public async Task<ActionResult<InventarioMaterialDto>> GetById(int id,CancellationToken ct)=>Ok(await mediator.Send(new GetInventarioMaterialByIdQuery(id),ct));
 [HttpPost][Authorize(Policy=AuthorizationPolicies.InventoryManagement)] public async Task<ActionResult<int>> Create([FromBody] MaterialRequest r,CancellationToken ct){var id=await mediator.Send(r.Create(),ct);return CreatedAtAction(nameof(GetById),new{id},id);}
 [HttpPut("{id:int}")][Authorize(Policy=AuthorizationPolicies.InventoryManagement)] public async Task<IActionResult> Update(int id,[FromBody] MaterialRequest r,CancellationToken ct){await mediator.Send(r.Update(id),ct);return NoContent();}
 [HttpDelete("{id:int}")][Authorize(Policy=AuthorizationPolicies.InventoryManagement)] public async Task<IActionResult> Delete(int id,CancellationToken ct){await mediator.Send(new DeleteInventarioMaterialCommand(id),ct);return NoContent();}
 public sealed record MaterialRequest(string Descripcion,string? MarcaModelo,int Cantidad,int CategoriaId,int EstadoId,DateOnly? FechaAdquisicion,string AdquiridoDonado,string? Observaciones,string? NumeroSerie,bool Activo=true){public CreateInventarioMaterialCommand Create()=>new(Descripcion,MarcaModelo,Cantidad,CategoriaId,EstadoId,FechaAdquisicion,AdquiridoDonado,Observaciones,NumeroSerie,Activo);public UpdateInventarioMaterialCommand Update(int id)=>new(id,Descripcion,MarcaModelo,Cantidad,CategoriaId,EstadoId,FechaAdquisicion,AdquiridoDonado,Observaciones,NumeroSerie,Activo);}
}
#pragma warning restore CS1591
