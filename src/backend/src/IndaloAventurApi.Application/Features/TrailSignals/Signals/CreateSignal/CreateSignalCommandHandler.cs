using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Domain.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignal;

public sealed class CreateSignalCommandHandler(
    ISignalRepository signalRepository,
    ISignalTypeRepository signalTypeRepository,
    ISignalImageStorage signalImageStorage) : IRequestHandler<CreateSignalCommand, Guid>
{
    public async Task<Guid> Handle(CreateSignalCommand request, CancellationToken cancellationToken)
    {
        if (!await signalTypeRepository.ExistsAsync(request.Tipo, cancellationToken))
        {
            throw new InvalidOperationException("El tipo de senal indicado no existe.");
        }

        var signalId = Guid.NewGuid();
        var storedImages = await signalImageStorage.SaveAsync(signalId, request.Foto1, request.Foto2 ?? [], cancellationToken);

        var signal = Signal.Crear(
            signalId,
            request.Latitud,
            request.Longitud,
            request.Titulo,
            request.Descripcion,
            storedImages.Foto1Path,
            storedImages.Foto2Path,
            request.Activo,
            request.UserIdAlta,
            request.Tipo,
            request.Tags);

        try
        {
            await signalRepository.AddAsync(signal, cancellationToken);
            await signalRepository.SaveChangesAsync(cancellationToken);
            return signal.Id;
        }
        catch
        {
            await signalImageStorage.DeleteAsync(storedImages.Foto1Path, storedImages.Foto2Path, cancellationToken);
            throw;
        }
    }
}
