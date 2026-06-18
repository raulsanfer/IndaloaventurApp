using IndaloAventurApi.Domain.ClubPositions;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Configurations;

public sealed class CargoConfiguration : IEntityTypeConfiguration<Cargo>
{
    public void Configure(EntityTypeBuilder<Cargo> builder)
    {
        builder.ToTable("Cargos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Descripcion).IsUnique();
    }
}
