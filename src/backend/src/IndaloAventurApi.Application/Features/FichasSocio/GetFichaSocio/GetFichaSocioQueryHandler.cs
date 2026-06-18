using IndaloAventurApi.Application.Abstractions.ClubPositions;
using IndaloAventurApi.Application.Abstractions.FichasSocio;
using IndaloAventurApi.Application.Common;
using MediatR;

namespace IndaloAventurApi.Application.Features.FichasSocio.GetFichaSocio;

public sealed class GetFichaSocioQueryHandler(IFichaSocioRepository repository, ICargoRepository cargoRepository) : IRequestHandler<GetFichaSocioQuery, FichaSocioDto>
{
    public async Task<FichaSocioDto> Handle(GetFichaSocioQuery request, CancellationToken cancellationToken)
    {
        if (!request.IsAdmin && request.AuthenticatedUserId != request.TargetUserId)
        {
            throw new ForbiddenAccessException("No tienes permisos para consultar esta ficha de socio.");
        }

        var ficha = await repository.GetByUserIdAsync(request.TargetUserId, cancellationToken);
        if (ficha is null)
        {
            throw new KeyNotFoundException("La ficha de socio no existe.");
        }
        
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
