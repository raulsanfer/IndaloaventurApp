using IndaloAventurApi.Domain.FichasSocio;
using IndaloAventurApi.Domain.ClubPositions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Configurations;

public sealed class FichaSocioConfiguration : IEntityTypeConfiguration<FichaSocio>
{
    public void Configure(EntityTypeBuilder<FichaSocio> builder)
    {
        builder.ToTable("FichasSocio");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.UserId).IsRequired();
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.CargoId).IsRequired(false);
        builder.HasIndex(x => x.CargoId);
        builder.HasOne<Cargo>()
            .WithMany()
            .HasForeignKey(x => x.CargoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Apellidos).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Dni).HasMaxLength(9).IsRequired();
        builder.Property(x => x.FechaNacimiento).IsRequired();
        builder.Property(x => x.Direccion).HasMaxLength(250).IsRequired();
        builder.Property(x => x.CodigoPostal).HasMaxLength(5).IsRequired();
        builder.Property(x => x.Poblacion).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Provincia).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Tlf).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(254).IsRequired();
        builder.Property(x => x.Alergias).HasMaxLength(1000).IsRequired(false);

        builder.Property(x => x.AceptaPoliticaPrivacidad).IsRequired();
        builder.Property(x => x.AceptaUsoImagenes).IsRequired();
        builder.Property(x => x.AceptaCobroCuenta).IsRequired();
    }
}
