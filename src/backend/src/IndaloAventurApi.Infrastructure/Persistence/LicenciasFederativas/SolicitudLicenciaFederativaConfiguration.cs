using IndaloAventurApi.Domain.LicenciasFederativas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;

public sealed class SolicitudLicenciaFederativaConfiguration : IEntityTypeConfiguration<SolicitudLicenciaFederativa>
{
    public void Configure(EntityTypeBuilder<SolicitudLicenciaFederativa> builder)
    {
        builder.ToTable("SolicitudesLicenciasFederativas", table =>
            table.HasCheckConstraint("CK_SolicitudesLicenciasFederativas_Cobertura",
                "([FechaInicio] IS NULL AND [FechaFin] IS NULL) OR ([FechaInicio] IS NOT NULL AND [FechaFin] IS NOT NULL AND [FechaInicio] <= [FechaFin])"));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Temporada).IsRequired();
        builder.Property(x => x.TarifaLicenciaFederativaId).IsRequired();
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();
        builder.Property(x => x.FechaCreacionUtc).IsRequired();

        builder.Property(x => x.FechaInicio).HasColumnType("date");
        builder.Property(x => x.FechaFin).HasColumnType("date");
        builder.HasIndex(x => new { x.UserId, x.Temporada });
        builder.HasIndex(x => x.TarifaLicenciaFederativaId);

        builder.HasOne(x => x.TarifaLicenciaFederativa)
            .WithMany()
            .HasForeignKey(x => x.TarifaLicenciaFederativaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
