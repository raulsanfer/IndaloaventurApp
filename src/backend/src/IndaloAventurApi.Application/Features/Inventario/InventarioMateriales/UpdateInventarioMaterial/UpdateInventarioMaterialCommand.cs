using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
namespace IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.UpdateInventarioMaterial;
public sealed record UpdateInventarioMaterialCommand(int Id,string Descripcion,string? MarcaModelo,int Cantidad,int CategoriaId,int EstadoId,DateOnly? FechaAdquisicion,string AdquiridoDonado,string? Observaciones,string? NumeroSerie,bool Activo) : ICommand<bool>;
public sealed class UpdateInventarioMaterialCommandValidator : AbstractValidator<UpdateInventarioMaterialCommand> { public UpdateInventarioMaterialCommandValidator() { RuleFor(x=>x.Id).GreaterThan(0); InventarioMaterialCommandRules.Apply(this,x=>x.Descripcion,x=>x.MarcaModelo,x=>x.Cantidad,x=>x.CategoriaId,x=>x.EstadoId,x=>x.FechaAdquisicion,x=>x.AdquiridoDonado,x=>x.Observaciones,x=>x.NumeroSerie); } }
