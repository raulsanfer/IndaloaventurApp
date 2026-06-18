using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Phonebook;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.GetFichaContactoById;

public sealed record GetFichaContactoByIdQuery(Guid Id) : IQuery<FichaContactoDto>;

public sealed class GetFichaContactoByIdQueryValidator : AbstractValidator<GetFichaContactoByIdQuery>
{
    public GetFichaContactoByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}