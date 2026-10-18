using IndaloAventurApi.Application.Abstractions.Inventario;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.DeleteInventarioEstado;

public sealed class DeleteInventarioEstadoCommandHandler(IInventarioEstadoRepository repository) : IRequestHandler<DeleteInventarioEstadoCommand, bool>
{
    public async Task<bool> Handle(DeleteInventarioEstadoCommand request, CancellationToken cancellationToken)
    {
        var estado = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("El estado de inventario no existe.");

        repository.Remove(estado);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
