using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.DeleteFichaContacto;

public sealed record DeleteFichaContactoCommand(Guid Id) : ICommand<bool>;

public sealed class DeleteFichaContactoCommandValidator : AbstractValidator<DeleteFichaContactoCommand>
{
    public DeleteFichaContactoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}