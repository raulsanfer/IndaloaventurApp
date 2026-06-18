using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Features.Users.CreateManagedUser;
using IndaloAventurApi.Application.Features.Users.DeactivateManagedUser;
using IndaloAventurApi.Application.Features.Users.ListManagedUsers;
using IndaloAventurApi.Application.Features.Users.ReactivateManagedUser;
using IndaloAventurApi.Application.Features.Users.UpdateManagedUser;

namespace IndaloAventurApi.Application.Tests.Users;

public sealed class UserManagementHandlersTests
{
    [Fact]
    public async Task ListManagedUsers_ShouldReturnUsersFromIdentityService()
    {
        var expected = new[]
        {
            new ManagedUserDto(Guid.NewGuid(), "member@club.test", true, ["Member"])
        };
        var identityService = new FakeIdentityService
        {
            ListUsersResult = expected
        };
        var handler = new ListManagedUsersQueryHandler(identityService);

        var result = await handler.Handle(new ListManagedUsersQuery(), CancellationToken.None);

        Assert.Same(expected, result);
        Assert.Null(identityService.LastListUsersEmail);
    }

    [Fact]
    public async Task ListManagedUsers_ShouldTrimEmailFilter_BeforeCallingIdentityService()
    {
        var identityService = new FakeIdentityService();
        var handler = new ListManagedUsersQueryHandler(identityService);

        await handler.Handle(new ListManagedUsersQuery("  CLUB.TEST  "), CancellationToken.None);

        Assert.Equal("CLUB.TEST", identityService.LastListUsersEmail);
    }

    [Fact]
    public async Task ListManagedUsers_ShouldTreatWhitespaceEmail_AsNoFilter()
    {
        var identityService = new FakeIdentityService();
        var handler = new ListManagedUsersQueryHandler(identityService);

        await handler.Handle(new ListManagedUsersQuery("   "), CancellationToken.None);

        Assert.Null(identityService.LastListUsersEmail);
    }

    [Fact]
    public async Task CreateManagedUser_ShouldReturnParsedGuid_WhenIdentityServiceSucceeds()
    {
        var createdUserId = Guid.NewGuid();
        var identityService = new FakeIdentityService
        {
            CreateUserResult = (true, createdUserId, Array.Empty<string>())
        };
        var handler = new CreateManagedUserCommandHandler(identityService);

        var result = await handler.Handle(
            new CreateManagedUserCommand("new-user@club.test", "Test1234A", ["Member"]),
            CancellationToken.None);

        Assert.Equal(createdUserId, result);
    }

    [Fact]
    public async Task CreateManagedUser_ShouldThrow_WhenIdentityServiceFails()
    {
        var identityService = new FakeIdentityService
        {
            CreateUserResult = (false, Guid.Empty, ["email ya en uso"])
        };
        var handler = new CreateManagedUserCommandHandler(identityService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new CreateManagedUserCommand("duplicated@club.test", "Test1234A", ["Member"]),
            CancellationToken.None));

        Assert.Contains("email ya en uso", exception.Message);
    }

    [Fact]
    public async Task UpdateManagedUser_ShouldReturnTrue_WhenIdentityServiceSucceeds()
    {
        var identityService = new FakeIdentityService
        {
            UpdateUserResult = (true, Array.Empty<string>())
        };
        var handler = new UpdateManagedUserCommandHandler(identityService);

        var result = await handler.Handle(
            new UpdateManagedUserCommand(Guid.NewGuid(), "updated@club.test", true, ["Admin"]),
            CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateManagedUser_ShouldThrow_WhenIdentityServiceFails()
    {
        var identityService = new FakeIdentityService
        {
            UpdateUserResult = (false, ["usuario no encontrado"])
        };
        var handler = new UpdateManagedUserCommandHandler(identityService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new UpdateManagedUserCommand(Guid.NewGuid(), "missing@club.test", false, ["Member"]),
            CancellationToken.None));

        Assert.Contains("usuario no encontrado", exception.Message);
    }

    [Fact]
    public async Task DeactivateManagedUser_ShouldReturnTrue_WhenIdentityServiceSucceeds()
    {
        var identityService = new FakeIdentityService
        {
            DeactivateUserResult = (true, Array.Empty<string>())
        };
        var handler = new DeactivateManagedUserCommandHandler(identityService);

        var result = await handler.Handle(new DeactivateManagedUserCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task DeactivateManagedUser_ShouldThrow_WhenIdentityServiceFails()
    {
        var identityService = new FakeIdentityService
        {
            DeactivateUserResult = (false, ["no se puede desactivar"])
        };
        var handler = new DeactivateManagedUserCommandHandler(identityService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new DeactivateManagedUserCommand(Guid.NewGuid()),
            CancellationToken.None));

        Assert.Contains("no se puede desactivar", exception.Message);
    }

    [Fact]
    public async Task ReactivateManagedUser_ShouldReturnTrue_WhenIdentityServiceSucceeds()
    {
        var identityService = new FakeIdentityService
        {
            ReactivateUserResult = (true, Array.Empty<string>())
        };
        var handler = new ReactivateManagedUserCommandHandler(identityService);

        var result = await handler.Handle(new ReactivateManagedUserCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ReactivateManagedUser_ShouldThrow_WhenIdentityServiceFails()
    {
        var identityService = new FakeIdentityService
        {
            ReactivateUserResult = (false, ["no se puede reactivar"])
        };
        var handler = new ReactivateManagedUserCommandHandler(identityService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new ReactivateManagedUserCommand(Guid.NewGuid()),
            CancellationToken.None));

        Assert.Contains("no se puede reactivar", exception.Message);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (bool Succeeded, Guid UserId, IEnumerable<string> Errors) CreateUserResult { get; init; } = (true, Guid.NewGuid(), Array.Empty<string>());
        public IReadOnlyCollection<ManagedUserDto> ListUsersResult { get; init; } = Array.Empty<ManagedUserDto>();
        public string? LastListUsersEmail { get; private set; }
        public (bool Succeeded, IEnumerable<string> Errors) UpdateUserResult { get; init; } = (true, Array.Empty<string>());
        public (bool Succeeded, IEnumerable<string> Errors) DeactivateUserResult { get; init; } = (true, Array.Empty<string>());
        public (bool Succeeded, IEnumerable<string> Errors) ReactivateUserResult { get; init; } = (true, Array.Empty<string>());

        public Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(string email, string password, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));

        public Task<(bool Succeeded, Guid? UserId, IEnumerable<string> Roles, bool IsMember)> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
            => Task.FromResult((true, (Guid?)Guid.NewGuid(), Array.Empty<string>() as IEnumerable<string>, false));

        public Task<(bool Succeeded, Guid? UserId, string? Email, IEnumerable<string> Roles, bool IsMember, IEnumerable<string> Errors)> ValidateSocialLoginAsync(string provider, string token, CancellationToken cancellationToken)
            => Task.FromResult((true, (Guid?)Guid.NewGuid(), (string?)"social@club.test", Array.Empty<string>() as IEnumerable<string>, false, Array.Empty<string>() as IEnumerable<string>));

        public Task<bool> IsUserActiveAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(string email, string password, IEnumerable<string> roles, CancellationToken cancellationToken)
            => Task.FromResult(CreateUserResult);

        public Task<IReadOnlyCollection<ManagedUserDto>> ListUsersAsync(string? email, CancellationToken cancellationToken)
        {
            LastListUsersEmail = email;
            return Task.FromResult(ListUsersResult);
        }

        public Task<(bool Succeeded, IEnumerable<string> Errors)> SetIsMemberAsync(Guid userId, bool isMember, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));

        public Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserAsync(Guid userId, string email, bool isMember, IEnumerable<string> roles, CancellationToken cancellationToken)
            => Task.FromResult(UpdateUserResult);

        public Task<(bool Succeeded, IEnumerable<string> Errors)> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(DeactivateUserResult);

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(ReactivateUserResult);

        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken)
            => Task.FromResult<string?>(null);

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));
    }
}

