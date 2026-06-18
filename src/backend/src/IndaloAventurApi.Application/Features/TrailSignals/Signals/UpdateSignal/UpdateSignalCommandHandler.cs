using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Application.Common;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.UpdateSignal;

public sealed class UpdateSignalCommandHandler(
    ISignalRepository signalRepository) : IRequestHandler<UpdateSignalCommand, bool>
{
    public async Task<bool> Handle(UpdateSignalCommand request, CancellationToken cancellationToken)
    {
        var signal = await signalRepository.GetByIdAsync(request.Id, cancellationToken);
        if (signal is null)
        {
            throw new KeyNotFoundException("La senal no existe.");
        }

        if (signal.UserIdAlta != request.UserIdModificacion)
        {
            throw new ForbiddenAccessException("No tienes permisos para editar esta senal.");
        }

        signal.ActualizarContenidoPropio(
            request.Titulo,
            request.Descripcion,
            request.Activo,
            request.UserIdModificacion);

        await signalRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
