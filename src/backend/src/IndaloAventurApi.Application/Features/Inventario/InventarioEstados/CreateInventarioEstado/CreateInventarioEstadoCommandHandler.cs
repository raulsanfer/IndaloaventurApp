using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Domain.Inventario;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.CreateInventarioEstado;

public sealed class CreateInventarioEstadoCommandHandler(IInventarioEstadoRepository repository) : IRequestHandler<CreateInventarioEstadoCommand, int>
{
    public async Task<int> Handle(CreateInventarioEstadoCommand request, CancellationToken cancellationToken)
    {
        var estado = InventarioEstado.Crear(request.Nombre);
        if (await repository.ExistsWithNormalizedNameAsync(estado.NombreNormalizado, null, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un estado de inventario con ese nombre.");
        }

        await repository.AddAsync(estado, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return estado.Id;
    }
}
