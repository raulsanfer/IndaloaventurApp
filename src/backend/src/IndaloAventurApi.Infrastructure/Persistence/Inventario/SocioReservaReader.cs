using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.Inventario;

public sealed class SocioReservaReader(ApplicationDbContext dbContext) : ISocioReservaReader
{
    public Task<SocioReservaDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => dbContext.FichasSocio
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new SocioReservaDto(x.UserId, $"{x.Nombre} {x.Apellidos}".Trim(), x.Email))
            .SingleOrDefaultAsync(cancellationToken);
}
