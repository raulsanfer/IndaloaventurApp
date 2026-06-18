using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;
using IndaloAventurApi.Application.Features.LicenciasFederativas.GetTarifasLicenciasFederativas;
using IndaloAventurApi.Domain.LicenciasFederativas;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Application.Tests.LicenciasFederativas;

public sealed class LicenciasFederativasHandlersTests
{
    [Fact]
    public async Task GetTarifas_ShouldReturnBothVariants_WhenMediaTemporadaIsNotProvided()
    {
        var tarifas = new[]
        {
            TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia"),
            TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia", mediaTemporada: true)
        };
        var handler = new GetTarifasLicenciasFederativasQueryHandler(new FakeTarifaRepository(tarifas));

        var result = await handler.Handle(new GetTarifasLicenciasFederativasQuery(2026, null), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => !x.MediaTemporada);
        Assert.Contains(result, x => x.MediaTemporada);
    }

    [Fact]
    public async Task GetTarifas_ShouldFilterByMediaTemporada_WhenProvided()
    {
        var tarifas = new[]
        {
            TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia"),
            TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 25m, 65m, "Andalucia", mediaTemporada: true)
        };
        var handler = new GetTarifasLicenciasFederativasQueryHandler(new FakeTarifaRepository(tarifas));

        var result = await handler.Handle(new GetTarifasLicenciasFederativasQuery(2026, true), CancellationToken.None);

        var tarifa = Assert.Single(result);
        Assert.True(tarifa.MediaTemporada);
        Assert.Equal(25m, tarifa.PrecioClub);
    }

    [Fact]
    public async Task CreateSolicitud_ShouldSendNotificationEmail_WhenRequestIsCreated()
    {
        var tarifa = TarifaLicenciaFederativa.Crear(2026, "A", "Mayores", 45m, 65m, "Andalucia");
        var tarifaRepository = new FakeTarifaRepository([tarifa]);
        var solicitudRepository = new FakeSolicitudRepository();
        var emailSender = new FakeEmailSender();
        var handler = new CreateSolicitudLicenciaFederativaCommandHandler(
            tarifaRepository,
            solicitudRepository,
            emailSender,
            Options.Create(new FederativeLicenseRequestNotificationOptions()),
            NullLogger<CreateSolicitudLicenciaFederativaCommandHandler>.Instance);

        var result = await handler.Handle(
            new CreateSolicitudLicenciaFederativaCommand(
                Guid.NewGuid(),
                "solicitante@club.test",
                true,
                2026,
                tarifa.Id),
            CancellationToken.None);

        Assert.Equal("A", result.Licencia);
        var message = Assert.Single(emailSender.Messages);
        Assert.Equal("club@indaloaventura.com", message.To);
        Assert.Equal("Nueva solicitud de licencia federativa", message.Subject);
        Assert.Contains("solicitante@club.test", message.HtmlBody, StringComparison.Ordinal);
        Assert.Contains("solicitante@club.test", message.PlainTextBody, StringComparison.Ordinal);
    }

    private sealed class FakeTarifaRepository(IEnumerable<TarifaLicenciaFederativa> tarifas) : ITarifaLicenciaFederativaRepository
    {
        private readonly IReadOnlyCollection<TarifaLicenciaFederativa> _tarifas = tarifas.ToArray();

        public Task<TarifaLicenciaFederativa?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(_tarifas.SingleOrDefault(x => x.Id == id));

        public Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListAsync(int? temporada, bool? mediaTemporada, CancellationToken cancellationToken)
        {
            IEnumerable<TarifaLicenciaFederativa> query = _tarifas;

            if (temporada.HasValue)
            {
                query = query.Where(x => x.Temporada == temporada.Value);
            }

            if (mediaTemporada.HasValue)
            {
                query = query.Where(x => x.MediaTemporada == mediaTemporada.Value);
            }

            return Task.FromResult((IReadOnlyCollection<TarifaLicenciaFederativa>)query.ToArray());
        }

        public Task<IReadOnlyCollection<TarifaLicenciaFederativa>> ListByTemporadaAsync(int temporada, CancellationToken cancellationToken)
            => Task.FromResult((IReadOnlyCollection<TarifaLicenciaFederativa>)_tarifas.Where(x => x.Temporada == temporada).ToArray());

        public Task AddAsync(TarifaLicenciaFederativa tarifa, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FakeSolicitudRepository : ISolicitudLicenciaFederativaRepository
    {
        public SolicitudLicenciaFederativa? Stored { get; private set; }

        public Task<SolicitudLicenciaFederativa?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(Stored is not null && Stored.Id == id ? Stored : null);

        public Task<SolicitudLicenciaFederativa?> GetByUserIdAndTemporadaAsync(Guid userId, int temporada, CancellationToken cancellationToken)
            => Task.FromResult(
                Stored is not null && Stored.UserId == userId && Stored.Temporada == temporada
                    ? Stored
                    : null);

        public Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(
                (IReadOnlyCollection<SolicitudLicenciaFederativa>)(Stored is not null && Stored.UserId == userId
                    ? [Stored]
                    : Array.Empty<SolicitudLicenciaFederativa>()));

        public Task<IReadOnlyCollection<SolicitudLicenciaFederativa>> ListAsync(Guid? userId, int? temporada, EstadoSolicitudLicenciaFederativa? estado, CancellationToken cancellationToken)
        {
            IEnumerable<SolicitudLicenciaFederativa> items = Stored is null ? [] : [Stored];

            if (userId.HasValue)
            {
                items = items.Where(x => x.UserId == userId.Value);
            }

            if (temporada.HasValue)
            {
                items = items.Where(x => x.Temporada == temporada.Value);
            }

            if (estado.HasValue)
            {
                items = items.Where(x => x.Estado == estado.Value);
            }

            return Task.FromResult((IReadOnlyCollection<SolicitudLicenciaFederativa>)items.ToArray());
        }

        public Task AddAsync(SolicitudLicenciaFederativa solicitud, CancellationToken cancellationToken)
        {
            Stored = solicitud;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        public List<EmailMessage> Messages { get; } = [];

        public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
