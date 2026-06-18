using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.GetAllSignalTypes;

public sealed class GetAllSignalTypesQueryHandler(ISignalTypeRepository repository) : IRequestHandler<GetAllSignalTypesQuery, IReadOnlyCollection<SignalTypeDto>>
{
    public async Task<IReadOnlyCollection<SignalTypeDto>> Handle(GetAllSignalTypesQuery request, CancellationToken cancellationToken)
    {
        var signalTypes = await repository.GetAllAsync(cancellationToken);

        return signalTypes
            .Select(x => new SignalTypeDto(x.Id, x.Nombre, x.Icono))
            .ToArray();
    }
}