using FluentValidation;
using IndaloAventurApi.Application.Abstractions.FichasSocio;
using IndaloAventurApi.Application.Abstractions.ClubPositions;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Common;
using IndaloAventurApi.Application.Features.FichasSocio.CreateFichaSocio;
using IndaloAventurApi.Application.Features.FichasSocio.GetFichaSocio;
using IndaloAventurApi.Application.Features.FichasSocio.UpdateFichaSocio;
using IndaloAventurApi.Domain.FichasSocio;

namespace IndaloAventurApi.Application.Tests.FichasSocio;

public sealed class FichaSocioHandlersTests
{
    [Fact]
    public async Task Get_ShouldAllowOwner()
    {
        var userId = Guid.NewGuid();
        var repo = new FakeFichaSocioRepository(FichaSocio.Crear(userId, 1, "Ana", "Perez", "12345678Z", new DateOnly(1990, 1, 1), "Calle A", "04001", "Almeria", "Almeria", "+34600111222", "ana@test.com", null, true, false, false));
        var handler = new GetFichaSocioQueryHandler(repo, new FakeCargoRepository(exists: true, descripcion: "Presidenta"));

        var dto = await handler.Handle(new GetFichaSocioQuery(userId, userId, false), CancellationToken.None);

        Assert.Equal(userId, dto.UserId);
        Assert.Equal("Ana", dto.Nombre);
        Assert.Equal(1, dto.CargoId);
        Assert.Equal("Presidenta", dto.CargoLabel);
    }

    [Fact]
    public async Task Get_ShouldDenyThirdParty_WhenNotAdmin()
    {
        var ownerId = Guid.NewGuid();
        var authId = Guid.NewGuid();
        var repo = new FakeFichaSocioRepository(FichaSocio.Crear(ownerId, 1, "Ana", "Perez", "12345678Z", new DateOnly(1990, 1, 1), "Calle A", "04001", "Almeria", "Almeria", "+34600111222", "ana@test.com", null, true, false, false));
        var handler = new GetFichaSocioQueryHandler(repo, new FakeCargoRepository(exists: true));

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(new GetFichaSocioQuery(authId, ownerId, false), CancellationToken.None));
    }

    [Fact]
    public async Task Create_ShouldAllowAdmin()
    {
        var authId = Guid.NewGuid();
        var targetId = Guid.NewGuid();
        var repo = new FakeFichaSocioRepository(null);
        var identityService = new FakeIdentityService();
        var handler = new CreateFichaSocioCommandHandler(repo, new FakeCargoRepository(exists: true), identityService);

        var dto = await handler.Handle(CreateCreateCommand(authId, targetId, true), CancellationToken.None);

        Assert.Equal(targetId, dto.UserId);
        Assert.NotNull(repo.Stored);
        Assert.Equal(targetId, identityService.LastSetIsMemberUserId);
        Assert.True(identityService.LastSetIsMemberValue);
    }

    [Fact]
    public async Task Create_ShouldDenyMember()
    {
        var authId = Guid.NewGuid();
        var targetId = Guid.NewGuid();
        var repo = new FakeFichaSocioRepository(null);
        var identityService = new FakeIdentityService();
        var handler = new CreateFichaSocioCommandHandler(repo, new FakeCargoRepository(exists: true), identityService);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(CreateCreateCommand(authId, targetId, false), CancellationToken.None));
        Assert.Null(identityService.LastSetIsMemberUserId);
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenIdentityMembershipSyncFails()
    {
        var authId = Guid.NewGuid();
        var targetId = Guid.NewGuid();
        var repo = new FakeFichaSocioRepository(null);
        var identityService = new FakeIdentityService
        {
            SetIsMemberResult = (false, ["Usuario no encontrado."])
        };
        var handler = new CreateFichaSocioCommandHandler(repo, new FakeCargoRepository(exists: true), identityService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(CreateCreateCommand(authId, targetId, true), CancellationToken.None));

        Assert.Contains("Usuario no encontrado.", exception.Message);
        Assert.Null(repo.Stored);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenNotExists()
    {
        var userId = Guid.NewGuid();
        var repo = new FakeFichaSocioRepository(null);
        var handler = new UpdateFichaSocioCommandHandler(repo, new FakeCargoRepository(exists: true));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(CreateUpdateCommand(userId, userId, false), CancellationToken.None));
    }

    [Fact]
    public async Task Update_ShouldDenyThirdParty_WhenNotAdmin()
    {
        var ownerId = Guid.NewGuid();
        var authId = Guid.NewGuid();
        var repo = new FakeFichaSocioRepository(FichaSocio.Crear(ownerId, 1, "Ana", "Perez", "12345678Z", new DateOnly(1990, 1, 1), "Calle A", "04001", "Almeria", "Almeria", "+34600111222", "ana@test.com", null, true, false, false));
        var handler = new UpdateFichaSocioCommandHandler(repo, new FakeCargoRepository(exists: true));

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(CreateUpdateCommand(authId, ownerId, false), CancellationToken.None));
    }

    [Fact]
    public void UpdateValidator_ShouldRejectInvalidFields()
    {
        var validator = new UpdateFichaSocioCommandValidator();
        var invalidCommand = CreateUpdateCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            false,
            dni: "ABC",
            codigoPostal: "12",
            tlf: "12",
            email: "correo-invalido",
            fechaNacimiento: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        var result = validator.Validate(invalidCommand);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Dni");
        Assert.Contains(result.Errors, x => x.PropertyName == "CodigoPostal");
        Assert.Contains(result.Errors, x => x.PropertyName == "Tlf");
        Assert.Contains(result.Errors, x => x.PropertyName == "Email");
        Assert.Contains(result.Errors, x => x.PropertyName == "FechaNacimiento");
    }

    private static UpdateFichaSocioCommand CreateUpdateCommand(
        Guid authenticatedUserId,
        Guid targetUserId,
        bool isAdmin,
        string dni = "12345678Z",
        string codigoPostal = "04001",
        string tlf = "+34600111222",
        string email = "ana@test.com",
        DateOnly? fechaNacimiento = null)
    {
        return new UpdateFichaSocioCommand(
            authenticatedUserId,
            targetUserId,
            isAdmin,
            1,
            "Ana",
            "Perez",
            dni,
            fechaNacimiento ?? new DateOnly(1990, 1, 1),
            "Calle A",
            codigoPostal,
            "Almeria",
            "Almeria",
            tlf,
            email,
            null,
            true,
            true,
            false);
    }

    private static CreateFichaSocioCommand CreateCreateCommand(Guid authenticatedUserId, Guid targetUserId, bool isAdmin)
    {
        return new CreateFichaSocioCommand(
            authenticatedUserId,
            targetUserId,
            isAdmin,
            1,
            "Ana",
            "Perez",
            "12345678Z",
            new DateOnly(1990, 1, 1),
            "Calle A",
            "04001",
            "Almeria",
            "Almeria",
            "+34600111222",
            "ana@test.com",
            null,
            true,
            true,
            false);
    }

    [Fact]
    public async Task Update_ShouldThrow_WhenCargoDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var repo = new FakeFichaSocioRepository(FichaSocio.Crear(userId, 1, "Ana", "Perez", "12345678Z", new DateOnly(1990, 1, 1), "Calle A", "04001", "Almeria", "Almeria", "+34600111222", "ana@test.com", null, true, false, false));
        var handler = new UpdateFichaSocioCommandHandler(repo, new FakeCargoRepository(exists: false));

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(CreateUpdateCommand(userId, userId, false), CancellationToken.None));
    }

    private sealed class FakeFichaSocioRepository(FichaSocio? initial) : IFichaSocioRepository
    {
        public FichaSocio? Stored { get; private set; } = initial;

        public Task<FichaSocio?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(Stored is not null && Stored.UserId == userId ? Stored : null);

        public Task AddAsync(FichaSocio fichaSocio, CancellationToken cancellationToken)
        {
            Stored = fichaSocio;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public Guid? LastSetIsMemberUserId { get; private set; }
        public bool LastSetIsMemberValue { get; private set; }
        public (bool Succeeded, IEnumerable<string> Errors) SetIsMemberResult { get; init; } = (true, Array.Empty<string>());

        public Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(string email, string password, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));

        public Task<(bool Succeeded, Guid? UserId, IEnumerable<string> Roles, bool IsMember)> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
            => Task.FromResult((true, (Guid?)Guid.NewGuid(), Array.Empty<string>() as IEnumerable<string>, false));

        public Task<(bool Succeeded, Guid? UserId, string? Email, IEnumerable<string> Roles, bool IsMember, IEnumerable<string> Errors)> ValidateSocialLoginAsync(string provider, string token, CancellationToken cancellationToken)
            => Task.FromResult((true, (Guid?)Guid.NewGuid(), (string?)"social@club.test", Array.Empty<string>() as IEnumerable<string>, false, Array.Empty<string>() as IEnumerable<string>));

        public Task<bool> IsUserActiveAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(string email, string password, IEnumerable<string> roles, CancellationToken cancellationToken)
            => Task.FromResult((true, Guid.NewGuid(), Array.Empty<string>() as IEnumerable<string>));

        public Task<IReadOnlyCollection<ManagedUserDto>> ListUsersAsync(string? email, CancellationToken cancellationToken)
            => Task.FromResult((IReadOnlyCollection<ManagedUserDto>)Array.Empty<ManagedUserDto>());

        public Task<(bool Succeeded, IEnumerable<string> Errors)> SetIsMemberAsync(Guid userId, bool isMember, CancellationToken cancellationToken)
        {
            LastSetIsMemberUserId = userId;
            LastSetIsMemberValue = isMember;
            return Task.FromResult(SetIsMemberResult);
        }

        public Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserAsync(Guid userId, string email, bool isMember, IEnumerable<string> roles, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));

        public Task<(bool Succeeded, IEnumerable<string> Errors)> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));

        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken)
            => Task.FromResult<string?>(null);

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));
    }

    private sealed class FakeCargoRepository(bool exists, string? descripcion = null) : ICargoRepository
    {
        public Task AddAsync(Domain.ClubPositions.Cargo cargo, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<IReadOnlyCollection<Domain.ClubPositions.Cargo>> GetAllAsync(CancellationToken cancellationToken) => Task.FromResult((IReadOnlyCollection<Domain.ClubPositions.Cargo>)Array.Empty<Domain.ClubPositions.Cargo>());
        public Task<Domain.ClubPositions.Cargo?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            if (descripcion is null)
            {
                return Task.FromResult<Domain.ClubPositions.Cargo?>(null);
            }

            var cargo = Domain.ClubPositions.Cargo.Crear(descripcion);
            typeof(Domain.ClubPositions.Cargo).GetProperty(nameof(Domain.ClubPositions.Cargo.Id))!.SetValue(cargo, id);
            return Task.FromResult<Domain.ClubPositions.Cargo?>(cargo);
        }
        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) => Task.FromResult(exists);
        public Task<bool> IsAssignedToAnyUserAsync(int id, CancellationToken cancellationToken) => Task.FromResult(false);
        public void Remove(Domain.ClubPositions.Cargo cargo) { }
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
