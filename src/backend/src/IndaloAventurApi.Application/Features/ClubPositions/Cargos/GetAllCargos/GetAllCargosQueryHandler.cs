using IndaloAventurApi.Application.Abstractions.ClubPositions;
using MediatR;

namespace IndaloAventurApi.Application.Features.ClubPositions.Cargos.GetAllCargos;

public sealed class GetAllCargosQueryHandler(ICargoRepository cargoRepository) : IRequestHandler<GetAllCargosQuery, IReadOnlyCollection<CargoDto>>
{
    public async Task<IReadOnlyCollection<CargoDto>> Handle(GetAllCargosQuery request, CancellationToken cancellationToken)
    {
        var cargos = await cargoRepository.GetAllAsync(cancellationToken);
        return cargos.Select(x => new CargoDto(x.Id, x.Descripcion)).ToArray();
    }
}
