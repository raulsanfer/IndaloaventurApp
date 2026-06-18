using IndaloAventurApi.Application.Abstractions.FichasSocio;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Common;
using IndaloAventurApi.Domain.FichasSocio;
using MediatR;

namespace IndaloAventurApi.Application.Features.FichasSocio.CreateFichaSocio;

public sealed class CreateFichaSocioCommandHandler(
    IFichaSocioRepository repository,
    Abstractions.ClubPositions.ICargoRepository cargoRepository,
    IIdentityService identityService) : IRequestHandler<CreateFichaSocioCommand, FichaSocioDto>
{
    public async Task<FichaSocioDto> Handle(CreateFichaSocioCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsAdmin)
        {
            throw new ForbiddenAccessException("Solo un administrador puede crear fichas de socio.");
        }

        var existing = await repository.GetByUserIdAsync(request.TargetUserId, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Ya existe una ficha de socio para el usuario indicado.");
        }
        if (request.CargoId.HasValue && !await cargoRepository.ExistsAsync(request.CargoId.Value, cancellationToken))
        {
            throw new InvalidOperationException("El cargo indicado no existe.");
        }

        var updateMembershipResult = await identityService.SetIsMemberAsync(request.TargetUserId, true, cancellationToken);
        if (!updateMembershipResult.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", updateMembershipResult.Errors));
        }

        var ficha = FichaSocio.Crear(
            request.TargetUserId,
            request.CargoId,
            request.Nombre,
            request.Apellidos,
            request.Dni,
            request.FechaNacimiento,
            request.Direccion,
            request.CodigoPostal,
            request.Poblacion,
            request.Provincia,
            request.Tlf,
            request.Email,
            request.Alergias,
            request.AceptaPoliticaPrivacidad,
            request.AceptaUsoImagenes,
            request.AceptaCobroCuenta);

        await repository.AddAsync(ficha, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        
        string cargoLabel = string.Empty;
        if (ficha.CargoId != null)
        {
            var cargo = await cargoRepository.GetByIdAsync(ficha.CargoId.Value, cancellationToken);
            cargoLabel = cargo?.Descripcion ?? string.Empty;

        } 

        return new FichaSocioDto(
            ficha.UserId,
            ficha.CargoId,
            cargoLabel,
            ficha.Nombre,
            ficha.Apellidos,
            ficha.Dni,
            ficha.FechaNacimiento,
            ficha.Direccion,
            ficha.CodigoPostal,
            ficha.Poblacion,
            ficha.Provincia,
            ficha.Tlf,
            ficha.Email,
            ficha.Alergias,
            ficha.AceptaPoliticaPrivacidad,
            ficha.AceptaUsoImagenes,
            ficha.AceptaCobroCuenta);
    }
}
