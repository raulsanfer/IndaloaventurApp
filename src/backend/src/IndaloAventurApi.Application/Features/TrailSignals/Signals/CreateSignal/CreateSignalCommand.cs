using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignal;

public sealed record CreateSignalCommand(
    float Latitud,
    float Longitud,
    string Titulo,
    string Descripcion,
    byte[] Foto1,
    byte[]? Foto2,
    bool Activo,
    Guid UserIdAlta,
    int Tipo,
    string Tags) : ICommand<Guid>;

public sealed class CreateSignalCommandValidator : AbstractValidator<CreateSignalCommand>
{
    public const int MaxBytesPorFoto = 2 * 1024 * 1024;

    public CreateSignalCommandValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty();
        RuleFor(x => x.Foto1).NotNull().Must(x => x.Length > 0).WithMessage("Foto1 es obligatoria.").Must(x => x.Length <= MaxBytesPorFoto);
        RuleFor(x => x.Foto2).Must(x => x is null || x.Length <= MaxBytesPorFoto).WithMessage($"Foto2 no puede superar {MaxBytesPorFoto} bytes.");
        RuleFor(x => x.UserIdAlta).NotEmpty();
        RuleFor(x => x.Tipo).GreaterThan(0);
        RuleFor(x => x.Tags).NotEmpty();
    }
}
