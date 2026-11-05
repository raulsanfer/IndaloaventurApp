using IndaloAventurApi.Application.Abstractions.Inventario;
using MediatR;
namespace IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.DeleteInventarioMaterial;
public sealed class DeleteInventarioMaterialCommandHandler(IInventarioMaterialRepository r) : IRequestHandler<DeleteInventarioMaterialCommand,bool> { public async Task<bool> Handle(DeleteInventarioMaterialCommand x,CancellationToken ct){var m=await r.GetByIdAsync(x.Id,ct)??throw new KeyNotFoundException("El material de inventario no existe.");r.Remove(m);await r.SaveChangesAsync(ct);return true;} }
