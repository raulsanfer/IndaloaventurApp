using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Domain.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.CreateSignalType;

public sealed class CreateSignalTypeCommandHandler(ISignalTypeRepository repository) : IRequestHandler<CreateSignalTypeCommand, int>
{
    public async Task<int> Handle(CreateSignalTypeCommand request, CancellationToken cancellationToken)
    {
        var signalType = SignalType.Crear(request.Nombre, request.Icono);
        await repository.AddAsync(signalType, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return signalType.Id;
    }
}
