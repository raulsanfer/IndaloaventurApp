using IndaloAventurApi.Application.Abstractions.Inventario;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.DeleteInventarioCategoria;

public sealed class DeleteInventarioCategoriaCommandHandler(IInventarioCategoriaRepository repository) : IRequestHandler<DeleteInventarioCategoriaCommand, bool>
{
    public async Task<bool> Handle(DeleteInventarioCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("La categoria de inventario no existe.");

        if (await repository.HasChildrenAsync(categoria.Id, cancellationToken))
        {
            throw new InvalidOperationException("No se puede eliminar una categoria que contiene subcategorias.");
        }

        repository.Remove(categoria);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
