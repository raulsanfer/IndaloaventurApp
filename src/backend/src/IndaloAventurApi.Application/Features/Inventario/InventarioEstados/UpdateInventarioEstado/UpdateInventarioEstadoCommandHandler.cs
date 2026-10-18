using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Domain.Inventario;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.UpdateInventarioEstado;

public sealed class UpdateInventarioEstadoCommandHandler(IInventarioEstadoRepository repository) : IRequestHandler<UpdateInventarioEstadoCommand, bool>
{
    public async Task<bool> Handle(UpdateInventarioEstadoCommand request, CancellationToken cancellationToken)
    {
        var estado = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("El estado de inventario no existe.");

        var nombre = InventarioNombre.Crear(request.Nombre);
        if (await repository.ExistsWithNormalizedNameAsync(nombre.ValorNormalizado, estado.Id, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un estado de inventario con ese nombre.");
        }

        estado.Actualizar(request.Nombre);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
