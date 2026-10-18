using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Domain.Inventario;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.UpdateInventarioCategoria;

public sealed class UpdateInventarioCategoriaCommandHandler(IInventarioCategoriaRepository repository) : IRequestHandler<UpdateInventarioCategoriaCommand, bool>
{
    public async Task<bool> Handle(UpdateInventarioCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("La categoria de inventario no existe.");

        if (request.CategoriaPadreId == categoria.Id)
        {
            throw new InvalidOperationException("Una categoria no puede ser su propia categoria padre.");
        }

        if (request.CategoriaPadreId.HasValue)
        {
            var padre = await repository.GetByIdAsync(request.CategoriaPadreId.Value, cancellationToken)
                ?? throw new KeyNotFoundException("La categoria padre no existe.");

            if (padre.CategoriaPadreId.HasValue)
            {
                throw new InvalidOperationException("Una subcategoria no puede tener subcategorias.");
            }

            if (!categoria.CategoriaPadreId.HasValue && await repository.HasChildrenAsync(categoria.Id, cancellationToken))
            {
                throw new InvalidOperationException("No se puede mover una categoria que contiene subcategorias.");
            }
        }

        var nombre = InventarioNombre.Crear(request.Nombre);
        if (await repository.ExistsWithNormalizedNameAsync(nombre.ValorNormalizado, request.CategoriaPadreId, categoria.Id, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe una categoria con ese nombre en el mismo nivel.");
        }

        categoria.Actualizar(request.Nombre, request.CategoriaPadreId);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
