using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.CreateFichaContacto;

public sealed record CreateFichaContactoCommand(string Nombre, string Telefono1, string? Telefono2, string? Email, string? Direccion, string? Observaciones) : ICommand<Guid>;

public sealed class CreateFichaContactoCommandValidator : AbstractValidator<CreateFichaContactoCommand>
{
    public CreateFichaContactoCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty();
        RuleFor(x => x.Telefono1).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().MaximumLength(254).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Direccion).MaximumLength(250).When(x => !string.IsNullOrWhiteSpace(x.Direccion));
    }
}
