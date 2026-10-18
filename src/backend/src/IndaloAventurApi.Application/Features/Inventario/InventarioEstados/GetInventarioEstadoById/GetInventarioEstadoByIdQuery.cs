using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.GetInventarioEstadoById;

public sealed record GetInventarioEstadoByIdQuery(int Id) : IQuery<InventarioEstadoDto>;

public sealed class GetInventarioEstadoByIdQueryValidator : AbstractValidator<GetInventarioEstadoByIdQuery>
{
    public GetInventarioEstadoByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
