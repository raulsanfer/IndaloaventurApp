using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.DeleteSignalType;

public sealed class DeleteSignalTypeCommandHandler(ISignalTypeRepository repository) : IRequestHandler<DeleteSignalTypeCommand, bool>
{
    public async Task<bool> Handle(DeleteSignalTypeCommand request, CancellationToken cancellationToken)
    {
        var signalType = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (signalType is null)
        {
            throw new KeyNotFoundException("El tipo de señal no existe.");
        }

        repository.Remove(signalType);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}

