using IndaloAventurApi.Domain.LicenciasFederativas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Configurations;

public sealed class TarifaLicenciaFederativaConfiguration : IEntityTypeConfiguration<TarifaLicenciaFederativa>
{
    public void Configure(EntityTypeBuilder<TarifaLicenciaFederativa> builder)
    {
        builder.ToTable("TarifasLicenciasFederativas");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Temporada).IsRequired();
        builder.Property(x => x.Licencia).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Categoria).HasMaxLength(40).IsRequired();
        builder.Property(x => x.MediaTemporada).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.PrecioClub).HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.PrecioIndependiente).HasPrecision(10, 2).IsRequired(false);
        builder.Property(x => x.Territorio).HasMaxLength(200).IsRequired();

        builder.HasIndex(x => new { x.Temporada, x.Licencia, x.Categoria, x.MediaTemporada }).IsUnique();

        builder.HasData(CreateSeedData());
    }

    private static object[] CreateSeedData()
    {
        var tarifasBase = new[]
        {
            new TarifaSeed(1, 2026, "A", "Mayores", 45.0m, 65.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(2, 2026, "A", "Juveniles", 11.0m, 31.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(3, 2026, "A", "Infantiles", 11.0m, 31.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(4, 2026, "A+", "Mayores", 58.0m, 78.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(5, 2026, "A+", "Juveniles", 26.0m, 46.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(6, 2026, "A+", "Infantiles", 26.0m, 46.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(7, 2026, "A Familiar", "Mayores", 43.0m, 63.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(8, 2026, "A Familiar", "Juveniles", 9.0m, 29.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(9, 2026, "A Familiar", "Infantiles", 9.0m, 29.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(10, 2026, "A-IS Inclusion Social", "Mayores", 19.0m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(11, 2026, "A-IS Inclusion Social", "Juveniles", 8.5m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(12, 2026, "A-IS Inclusion Social", "Infantiles", 8.5m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(13, 2026, "A+IS Inclusion Social", "Mayores", 33.0m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(14, 2026, "A+IS Inclusion Social", "Juveniles", 23.0m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(15, 2026, "A+IS Inclusion Social", "Infantiles", 23.0m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(16, 2026, "A ESP", "Mayores", 28.0m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(17, 2026, "A65", "Mayores", 32.0m, 52.0m, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(18, 2026, "A REF", "Juveniles", 8.5m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(19, 2026, "A REF", "Infantiles", 8.5m, null, "Andalucia, Ceuta y Melilla"),
            new TarifaSeed(20, 2026, "A NAC", "Mayores", 52.0m, 72.0m, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos"),
            new TarifaSeed(21, 2026, "A NAC", "Juveniles", 22.0m, 42.0m, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos"),
            new TarifaSeed(22, 2026, "A NAC", "Infantiles", 22.0m, 42.0m, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos"),
            new TarifaSeed(23, 2026, "A NAC +", "Mayores", 69.0m, 89.0m, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos"),
            new TarifaSeed(24, 2026, "A NAC +", "Juveniles", 36.0m, 56.0m, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos"),
            new TarifaSeed(25, 2026, "A NAC +", "Infantiles", 36.0m, 56.0m, "Espana, Andorra, Pirineo Frances, Portugal y Marruecos")
        };

        return tarifasBase
            .SelectMany(tarifa => new object[]
            {
                new
                {
                    tarifa.Id,
                    tarifa.Temporada,
                    tarifa.Licencia,
                    tarifa.Categoria,
                    MediaTemporada = false,
                    tarifa.PrecioClub,
                    tarifa.PrecioIndependiente,
                    tarifa.Territorio
                },
                new
                {
                    Id = tarifa.Id + tarifasBase.Length,
                    tarifa.Temporada,
                    tarifa.Licencia,
                    tarifa.Categoria,
                    MediaTemporada = true,
                    tarifa.PrecioClub,
                    tarifa.PrecioIndependiente,
                    tarifa.Territorio
                }
            })
            .ToArray();
    }

    private sealed record TarifaSeed(
        int Id,
        int Temporada,
        string Licencia,
        string Categoria,
        decimal PrecioClub,
        decimal? PrecioIndependiente,
        string Territorio);
}
