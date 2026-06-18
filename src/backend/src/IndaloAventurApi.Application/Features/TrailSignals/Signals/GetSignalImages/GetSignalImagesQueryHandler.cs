using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalImages;

public sealed class GetSignalImagesQueryHandler(ISignalRepository signalRepository) : IRequestHandler<GetSignalImagesQuery, SignalImagesDto>
{
    public async Task<SignalImagesDto> Handle(GetSignalImagesQuery request, CancellationToken cancellationToken)
    {
        var signal = await signalRepository.GetByIdAsync(request.SignalId, cancellationToken);
        if (signal is null)
        {
            throw new KeyNotFoundException("La senal no existe.");
        }

        return new SignalImagesDto(signal.Id, signal.Foto1, signal.Foto2);
    }
}