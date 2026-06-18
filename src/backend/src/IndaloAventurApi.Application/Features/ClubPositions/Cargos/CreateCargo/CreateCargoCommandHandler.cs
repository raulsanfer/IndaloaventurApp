using IndaloAventurApi.Application.Abstractions.ClubPositions;
using IndaloAventurApi.Domain.ClubPositions;
using MediatR;

namespace IndaloAventurApi.Application.Features.ClubPositions.Cargos.CreateCargo;

public sealed class CreateCargoCommandHandler(ICargoRepository cargoRepository) : IRequestHandler<CreateCargoCommand, int>
{
    public async Task<int> Handle(CreateCargoCommand request, CancellationToken cancellationToken)
    {
        var cargo = Cargo.Crear(request.Descripcion);
        await cargoRepository.AddAsync(cargo, cancellationToken);
        await cargoRepository.SaveChangesAsync(cancellationToken);
        return cargo.Id;
    }
}
