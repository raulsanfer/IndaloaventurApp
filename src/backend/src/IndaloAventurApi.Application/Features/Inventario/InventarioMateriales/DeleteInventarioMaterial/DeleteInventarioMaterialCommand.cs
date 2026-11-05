using IndaloAventurApi.Application.Abstractions.Cqrs;
namespace IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.DeleteInventarioMaterial;
public sealed record DeleteInventarioMaterialCommand(int Id) : ICommand<bool>;
