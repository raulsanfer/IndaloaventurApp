using IndaloAventurApi.Application.Abstractions.ClubPositions;
using MediatR;

namespace IndaloAventurApi.Application.Features.ClubPositions.Cargos.DeleteCargo;

public sealed class DeleteCargoCommandHandler(ICargoRepository cargoRepository) : IRequestHandler<DeleteCargoCommand, bool>
{
    public async Task<bool> Handle(DeleteCargoCommand request, CancellationToken cancellationToken)
    {
        var cargo = await cargoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cargo is null)
        {
            throw new KeyNotFoundException("El cargo no existe.");
        }

        if (await cargoRepository.IsAssignedToAnyUserAsync(request.Id, cancellationToken))
        {
            throw new InvalidOperationException("No se puede eliminar un cargo asignado a usuarios.");
        }

        cargoRepository.Remove(cargo);
        await cargoRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
