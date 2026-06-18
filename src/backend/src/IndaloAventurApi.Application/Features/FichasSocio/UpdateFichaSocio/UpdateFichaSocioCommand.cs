using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.FichasSocio;

namespace IndaloAventurApi.Application.Features.FichasSocio.UpdateFichaSocio;

public sealed record UpdateFichaSocioCommand(
    Guid AuthenticatedUserId,
    Guid TargetUserId,
    bool IsAdmin,
    int? CargoId,
    string Nombre,
    string Apellidos,
    string Dni,
    DateOnly FechaNacimiento,
    string Direccion,
    string CodigoPostal,
    string Poblacion,
    string Provincia,
    string Tlf,
    string Email,
    string? Alergias,
    bool AceptaPoliticaPrivacidad,
    bool AceptaUsoImagenes,
    bool AceptaCobroCuenta) : ICommand<FichaSocioDto>;

public sealed class UpdateFichaSocioCommandValidator : AbstractValidator<UpdateFichaSocioCommand>
{
    public UpdateFichaSocioCommandValidator()
    {
        RuleFor(x => x.AuthenticatedUserId).NotEmpty();
        RuleFor(x => x.TargetUserId).NotEmpty();
        RuleFor(x => x.CargoId).GreaterThan(0).When(x => x.CargoId.HasValue);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Dni).NotEmpty().Matches("^[0-9]{8}[A-Za-z]$").WithMessage("El DNI debe tener 8 digitos y 1 letra.");
        RuleFor(x => x.FechaNacimiento)
            .Must(x => x <= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("La fecha de nacimiento no puede ser futura.");
        RuleFor(x => x.Direccion).NotEmpty().MaximumLength(250);
        RuleFor(x => x.CodigoPostal).NotEmpty().Matches("^[0-9]{5}$").WithMessage("El codigo postal debe tener 5 digitos.");
        RuleFor(x => x.Poblacion).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Provincia).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Tlf).NotEmpty().Matches(@"^\+?[0-9]{9,15}$").WithMessage("El telefono debe contener entre 9 y 15 digitos.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Alergias).MaximumLength(1000).When(x => x.Alergias is not null);
        RuleFor(x => x.AceptaPoliticaPrivacidad)
            .Equal(true)
            .WithMessage("Es obligatorio aceptar la politica de privacidad.");
    }
}
