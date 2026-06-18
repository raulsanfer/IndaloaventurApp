using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.UpdateSignalType;

public sealed class UpdateSignalTypeCommandHandler(ISignalTypeRepository repository) : IRequestHandler<UpdateSignalTypeCommand, bool>
{
    public async Task<bool> Handle(UpdateSignalTypeCommand request, CancellationToken cancellationToken)
    {
        var signalType = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (signalType is null)
        {
            throw new KeyNotFoundException("El tipo de señal no existe.");
        }

        signalType.Actualizar(request.Nombre, request.Icono);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}

