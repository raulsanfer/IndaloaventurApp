using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Common;
using IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetTarifasLicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Tests.LicenciasFederativas;

public sealed class LicenciasFederativasHandlersTests
{
    private static readonly CancellationToken Token = CancellationToken.None;

    [Theory]
    [InlineData(null, null, 3)]
    [InlineData(2026, null, 3)]
    [InlineData(null, 2, 1)]
    [InlineData(2026, 3, 1)]
    [InlineData(2030, 1, 0)]
    public async Task Tarifas_FiltraPorTemporadaYTipo(int? temporada, int? tipo, int cantidad)
    {
        var handler = new GetTarifasLicenciasFederativasQueryHandler(new FakeTarifas([Tarifa(1), Tarifa(2), Tarifa(3)]));
        var result = await handler.Handle(new(temporada, tipo), Token);
        Assert.Equal(cantidad, result.Count);
        Assert.All(result, x => Assert.False(string.IsNullOrWhiteSpace(x.TipoLicenciaNombre)));
    }

    [Theory]
    [InlineData(1, 2, false)]
    [InlineData(2, 1, false)]
    [InlineData(1, 1, false)]
    [InlineData(3, 3, true)]
    [InlineData(3, 1, true)]
    [InlineData(1, 3, true)]
    public async Task Alta_AplicaMultiplicidadPorTipo(int anterior, int nuevo, bool permitido)
    {
        var user = Guid.NewGuid();
        var repo = new FakeSolicitudes();
        await repo.AddAsync(SolicitudLicenciaFederativa.Crear(user, 2026, Tarifa(anterior),
            anterior == 3 ? new DateOnly(2026, 4, 1) : null, anterior == 3 ? new DateOnly(2026, 4, 1) : null), Token);
        repo.Stored[0].Cancelar(); // Cancellation continues to count for the non-daily limit.
        var email = new FakeEmail(repo);
        var command = new CreateSolicitudLicenciaFederativaCommand(user, "socio@club.test", true, 2026, nuevo,
            nuevo == 3 ? new DateOnly(2026, 5, 1) : null, nuevo == 3 ? new DateOnly(2026, 5, 2) : null);
        if (permitido)
        {
            var dto = await Handler(Tarifa(nuevo), repo, email).Handle(command, Token);
            Assert.Equal(2, repo.Stored.Count);
            Assert.Equal(nuevo, dto.TipoLicenciaId);
            Assert.Equal(command.FechaInicio, dto.FechaInicio);
            Assert.Single(email.Messages);
        }
        else
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => Handler(Tarifa(nuevo), repo, email).Handle(command, Token));
            Assert.Single(repo.Stored);
            Assert.Empty(email.Messages);
        }
    }

    [Fact]
    public async Task Alta_NotificaDespuesDelCommitYUnFalloDeCorreoNoRevierte()
    {
        var repo = new FakeSolicitudes();
        var email = new FakeEmail(repo) { Fail = true };
        var result = await Handler(Tarifa(1), repo, email).Handle(new(Guid.NewGuid(), "socio@club.test", true, 2026, 1), Token);
        Assert.Equal(Assert.Single(repo.Stored).Id, result.Id);
        var message = Assert.Single(email.Messages);
        Assert.Equal("club@indaloaventura.com", message.To);
        Assert.Contains("socio@club.test", message.HtmlBody);
        Assert.Contains("socio@club.test", message.PlainTextBody);
    }

    [Fact]
    public async Task Alta_NoNotificaSiFallaLaPersistencia()
    {
        var repo = new FakeSolicitudes { FailSave = true };
        var email = new FakeEmail(repo);
        await Assert.ThrowsAsync<InvalidOperationException>(() => Handler(Tarifa(1), repo, email)
            .Handle(new(Guid.NewGuid(), "socio@club.test", true, 2026, 1), Token));
        Assert.Empty(email.Messages);
        Assert.False(repo.Committed);
    }

    [Fact]
    public async Task Alta_RechazaNoSocioTarifaInexistenteTemporadaIncoherenteYCoberturaDiariaAusente()
    {
        var repo = new FakeSolicitudes();
        var email = new FakeEmail(repo);
        var handler = Handler(Tarifa(3), repo, email);
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(new(Guid.NewGuid(), "socio@club.test", false, 2026, 3), Token));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new(Guid.NewGuid(), "socio@club.test", true, 2026, 999), Token));
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new(Guid.NewGuid(), "socio@club.test", true, 2027, 3), Token));
        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(new(Guid.NewGuid(), "socio@club.test", true, 2026, 3), Token));
        Assert.Empty(repo.Stored);
        Assert.Empty(email.Messages);
    }

    [Fact]
    public void Validadores_RechazanTipoNoPositivoYCoberturaIncompleta()
    {
        Assert.False(new GetTarifasLicenciasFederativasQueryValidator().Validate(new GetTarifasLicenciasFederativasQuery(2026, 0)).IsValid);
        Assert.False(new CreateSolicitudLicenciaFederativaCommandValidator().Validate(
            new CreateSolicitudLicenciaFederativaCommand(Guid.NewGuid(), "socio@club.test", true, 2026, 1, new DateOnly(2026, 1, 1))).IsValid);
    }

    private static TarifaLicenciaFederativa Tarifa(int tipo)
    {
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, null, "Andalucía", tipo);
        typeof(TarifaLicenciaFederativa).GetProperty(nameof(tarifa.Id))!.SetValue(tarifa, tipo);
        typeof(TarifaLicenciaFederativa).GetProperty(nameof(tarifa.TipoLicencia))!.SetValue(tarifa,
            TipoLicencia.Crear(tipo, tipo switch { 1 => "Completa", 2 => "Media 01/07 - 31/12", _ => "Día" }));
        return tarifa;
    }

    private static CreateSolicitudLicenciaFederativaCommandHandler Handler(TarifaLicenciaFederativa tarifa, FakeSolicitudes repo, FakeEmail email)
        => new(new FakeTarifas([tarifa]), repo, email, Options.Create(new FederativeLicenseRequestNotificationOptions()),
            NullLogger<CreateSolicitudLicenciaFederativaCommandHandler>.Instance);

    private sealed class FakeTarifas(TarifaLicenciaFederativa[] tarifas) : ITarifaLicenciaFederativaRepository
    {
        public Task<TarifaLicenciaFederativa?> GetByIdAsync(int id, CancellationToken token) => Task.FromResult(tarifas.SingleOrDefault(x => x.Id == id));
        public Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListAsync(int? temporada, int? tipo, CancellationToken token)
            => Task.FromResult<IReadOnlyCollection<TarifaLicenciaFederativa>>(tarifas.Where(x => (!temporada.HasValue || x.Temporada == temporada) && (!tipo.HasValue || x.TipoLicenciaId == tipo)).ToArray());
        public Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListByTemporadaAsync(int temporada, CancellationToken token) => ListAsync(temporada, null, token);
        public Task AddAsync(TarifaLicenciaFederativa tarifa, CancellationToken token) => Task.CompletedTask;
        public Task SaveChangesAsync(CancellationToken token) => Task.CompletedTask;
    }

    private sealed class FakeSolicitudes : ISolicitudLicenciaFederativaRepository
    {
        public List<SolicitudLicenciaFederativa> Stored { get; } = [];
        public bool Committed { get; private set; }
        public bool FailSave { get; init; }
        public Task<SolicitudLicenciaFederativa?> GetByIdAsync(Guid id, CancellationToken token) => Task.FromResult(Stored.SingleOrDefault(x => x.Id == id));
        public Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListByUserIdAndTemporadaForUpdateAsync(Guid user, int temporada, CancellationToken token)
            => Task.FromResult<IReadOnlyCollection<SolicitudLicenciaFederativa>>(Stored.Where(x => x.UserId == user && x.Temporada == temporada).ToArray());
        public Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListByUserIdAsync(Guid user, CancellationToken token)
            => Task.FromResult<IReadOnlyCollection<SolicitudLicenciaFederativa>>(Stored.Where(x => x.UserId == user).ToArray());
        public async Task<T> ExecuteSerializableAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken token)
        {
            var result = await operation(token);
            Committed = true;
            return result;
        }
        public Task AddAsync(SolicitudLicenciaFederativa solicitud, CancellationToken token)
        {
            typeof(SolicitudLicenciaFederativa).GetProperty(nameof(solicitud.TarifaLicenciaFederativa))!.SetValue(solicitud, Tarifa(solicitud.TarifaLicenciaFederativaId));
            Stored.Add(solicitud);
            return Task.CompletedTask;
        }
        public Task SaveChangesAsync(CancellationToken token) => FailSave ? throw new InvalidOperationException("Fallo simulado") : Task.CompletedTask;
    }

    private sealed class FakeEmail(FakeSolicitudes repo) : IEmailSender
    {
        public bool Fail { get; init; }
        public List<EmailMessage> Messages { get; } = [];
        public Task SendAsync(EmailMessage message, CancellationToken token)
        {
            Assert.True(repo.Committed);
            Messages.Add(message);
            return Fail ? throw new InvalidOperationException("Fallo de correo simulado") : Task.CompletedTask;
        }
    }
}
