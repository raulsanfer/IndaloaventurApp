using IndaloAventurApi.Domain.LicenciasFederativas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;

public sealed class TipoLicenciaConfiguration : IEntityTypeConfiguration<TipoLicencia>
{
    public void Configure(EntityTypeBuilder<TipoLicencia> builder)
    {
        builder.ToTable("TiposLicencias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Nombre).HasMaxLength(80).IsRequired();
        builder.HasIndex(x => x.Nombre).IsUnique();
        builder.HasData(
            TipoLicencia.Crear(TipoLicencia.CompletaId, "Completa"),
            TipoLicencia.Crear(TipoLicencia.MediaId, "Media 01/07 - 31/12"),
            TipoLicencia.Crear(TipoLicencia.DiaId, "Día"));
    }
}
