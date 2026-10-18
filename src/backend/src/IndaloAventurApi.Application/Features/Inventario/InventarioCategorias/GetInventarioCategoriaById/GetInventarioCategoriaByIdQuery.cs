using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.GetInventarioCategoriaById;

public sealed record GetInventarioCategoriaByIdQuery(int Id) : IQuery<InventarioCategoriaDto>;

public sealed class GetInventarioCategoriaByIdQueryValidator : AbstractValidator<GetInventarioCategoriaByIdQuery>
{
    public GetInventarioCategoriaByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
