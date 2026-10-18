using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Domain.Inventario;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.CreateInventarioCategoria;

public sealed class CreateInventarioCategoriaCommandHandler(IInventarioCategoriaRepository repository) : IRequestHandler<CreateInventarioCategoriaCommand, int>
{
    public async Task<int> Handle(CreateInventarioCategoriaCommand request, CancellationToken cancellationToken)
    {
        await ValidarPadreAsync(request.CategoriaPadreId, null, cancellationToken);

        var categoria = InventarioCategoria.Crear(request.Nombre, request.CategoriaPadreId);
        if (await repository.ExistsWithNormalizedNameAsync(categoria.NombreNormalizado, categoria.CategoriaPadreId, null, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe una categoria con ese nombre en el mismo nivel.");
        }

        await repository.AddAsync(categoria, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return categoria.Id;
    }

    private async Task ValidarPadreAsync(int? categoriaPadreId, int? categoriaId, CancellationToken cancellationToken)
    {
        if (!categoriaPadreId.HasValue)
        {
            return;
        }

        if (categoriaPadreId == categoriaId)
        {
            throw new InvalidOperationException("Una categoria no puede ser su propia categoria padre.");
        }

        var padre = await repository.GetByIdAsync(categoriaPadreId.Value, cancellationToken);
        if (padre is null)
        {
            throw new KeyNotFoundException("La categoria padre no existe.");
        }

        if (padre.CategoriaPadreId.HasValue)
        {
            throw new InvalidOperationException("Una subcategoria no puede tener subcategorias.");
        }
    }
}
