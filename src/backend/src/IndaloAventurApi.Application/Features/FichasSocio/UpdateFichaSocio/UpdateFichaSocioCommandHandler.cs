using IndaloAventurApi.Application.Abstractions.FichasSocio;
using IndaloAventurApi.Application.Common;
using MediatR;

namespace IndaloAventurApi.Application.Features.FichasSocio.UpdateFichaSocio;

public sealed class UpdateFichaSocioCommandHandler(
    IFichaSocioRepository repository,
    Abstractions.ClubPositions.ICargoRepository cargoRepository) : IRequestHandler<UpdateFichaSocioCommand, FichaSocioDto>
{
    public async Task<FichaSocioDto> Handle(UpdateFichaSocioCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsAdmin && request.AuthenticatedUserId != request.TargetUserId)
        {
            throw new ForbiddenAccessException("No tienes permisos para editar esta ficha de socio.");
        }

        var ficha = await repository.GetByUserIdAsync(request.TargetUserId, cancellationToken);
        if (ficha is null)
        {
            throw new KeyNotFoundException("La ficha de socio no existe.");
        }
        if (request.CargoId.HasValue && !await cargoRepository.ExistsAsync(request.CargoId.Value, cancellationToken))
        {
            throw new InvalidOperationException("El cargo indicado no existe.");
        }

        ficha.Actualizar(
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

        await repository.SaveChangesAsync(cancellationToken);

        string cargoLabel = string.Empty;

        if(ficha.CargoId != null)
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
