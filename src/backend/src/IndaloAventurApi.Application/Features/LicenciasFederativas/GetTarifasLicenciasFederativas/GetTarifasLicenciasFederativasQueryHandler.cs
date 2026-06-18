using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using MediatR;

namespace IndaloAventurApi.Application.Features.LicenciasFederativas.GetTarifasLicenciasFederativas;

public sealed class GetTarifasLicenciasFederativasQueryHandler(ITarifaLicenciaFederativaRepository tarifaRepository)
    : IRequestHandler<GetTarifasLicenciasFederativasQuery, IReadOnlyCollection<TarifaLicenciaFederativaDto>>
{
    public async Task<IReadOnlyCollection<TarifaLicenciaFederativaDto>> Handle(GetTarifasLicenciasFederativasQuery request, CancellationToken cancellationToken)
    {
        var tarifas = await tarifaRepository.ListAsync(request.Temporada, request.MediaTemporada, cancellationToken);

        return tarifas
            .Select(tarifa => new TarifaLicenciaFederativaDto(
                tarifa.Id,
                tarifa.Temporada,
                tarifa.Licencia,
                tarifa.Categoria,
                tarifa.Territorio,
                tarifa.MediaTemporada,
                tarifa.PrecioClub,
                tarifa.PrecioIndependiente))
            .ToArray();
    }
}
