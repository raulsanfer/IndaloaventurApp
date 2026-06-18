using IndaloAventurApi.Application.Abstractions.ClubPositions;
using MediatR;

namespace IndaloAventurApi.Application.Features.ClubPositions.Cargos.UpdateCargo;

public sealed class UpdateCargoCommandHandler(ICargoRepository cargoRepository) : IRequestHandler<UpdateCargoCommand, bool>
{
    public async Task<bool> Handle(UpdateCargoCommand request, CancellationToken cancellationToken)
    {
        var cargo = await cargoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cargo is null)
        {
            throw new KeyNotFoundException("El cargo no existe.");
        }

        cargo.Actualizar(request.Descripcion);
        await cargoRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
