using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Domain.ClubPositions;
using IndaloAventurApi.Domain.FichasContacto;
using IndaloAventurApi.Domain.FichasSocio;
using IndaloAventurApi.Domain.LicenciasFederativas;
using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using IndaloAventurApi.Domain.TrailSignals;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Persistence.DbContext;

public sealed class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<FichaContacto> FichasContacto => Set<FichaContacto>();
    public DbSet<FichaSocio> FichasSocio => Set<FichaSocio>();
    public DbSet<TarifaLicenciaFederativa> TarifasLicenciasFederativas => Set<TarifaLicenciaFederativa>();
    public DbSet<SolicitudLicenciaFederativa> SolicitudesLicenciasFederativas => Set<SolicitudLicenciaFederativa>();
    public DbSet<SignalType> SignalTypes => Set<SignalType>();
    public DbSet<Signal> Signals => Set<Signal>();
    public DbSet<SignalComment> SignalComments => Set<SignalComment>();
    public DbSet<InventarioCategoria> InventarioCategorias => Set<InventarioCategoria>();
    public DbSet<InventarioEstado> InventarioEstados => Set<InventarioEstado>();
    public DbSet<InventarioMaterial> InventarioMateriales => Set<InventarioMaterial>();
    public DbSet<ReservaMaterial> ReservasMaterial => Set<ReservaMaterial>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
