using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
namespace IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.CreateInventarioMaterial;
public sealed record CreateInventarioMaterialCommand(string Descripcion, string? MarcaModelo, int Cantidad, int CategoriaId, int EstadoId, DateOnly? FechaAdquisicion, string AdquiridoDonado, string? Observaciones, string? NumeroSerie, bool Activo = true) : ICommand<int>;
public sealed class CreateInventarioMaterialCommandValidator : AbstractValidator<CreateInventarioMaterialCommand> { public CreateInventarioMaterialCommandValidator() => InventarioMaterialCommandRules.Apply(this, x=>x.Descripcion,x=>x.MarcaModelo,x=>x.Cantidad,x=>x.CategoriaId,x=>x.EstadoId,x=>x.FechaAdquisicion,x=>x.AdquiridoDonado,x=>x.Observaciones,x=>x.NumeroSerie); }
