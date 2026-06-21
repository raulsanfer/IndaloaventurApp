using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalImages;

public sealed class GetSignalImagesQueryHandler(
    ISignalRepository signalRepository,
    ISignalImageStorage signalImageStorage) : IRequestHandler<GetSignalImagesQuery, SignalImagesDto>
{
    public async Task<SignalImagesDto> Handle(GetSignalImagesQuery request, CancellationToken cancellationToken)
    {
        var signal = await signalRepository.GetByIdAsync(request.SignalId, cancellationToken);
        if (signal is null)
        {
            throw new KeyNotFoundException("La senal no existe.");
        }

        var images = await signalImageStorage.ReadAsync(signal.Foto1Path, signal.Foto2Path, cancellationToken);
        return new SignalImagesDto(signal.Id, images.Foto1, images.Foto2);
    }
}
