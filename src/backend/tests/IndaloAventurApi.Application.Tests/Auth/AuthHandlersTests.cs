using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Application.Features.Auth.Login;
using IndaloAventurApi.Application.Features.Auth.SocialLogin;

namespace IndaloAventurApi.Application.Tests.Auth;

public sealed class AuthHandlersTests
{
    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var identityService = new FakeIdentityService
        {
            ValidateCredentialsResult = (true, Guid.Parse("11111111-1111-1111-1111-111111111111"), ["Member"], true)
        };
        var tokenService = new FakeJwtTokenService();
        var handler = new LoginQueryHandler(identityService, tokenService);

        var result = await handler.Handle(new LoginQuery("member@club.test", "Test1234A"), CancellationToken.None);

        Assert.Equal("fake-token", result.AccessToken);
        Assert.True(result.IsMember);
        Assert.True(tokenService.LastIsMember);
    }

    [Fact]
    public async Task Login_ShouldThrowUnauthorized_WhenCredentialsAreInvalid()
    {
        var identityService = new FakeIdentityService
        {
            ValidateCredentialsResult = (false, null, Array.Empty<string>(), false)
        };
        var handler = new LoginQueryHandler(identityService, new FakeJwtTokenService());

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(new LoginQuery("member@club.test", "Test1234A"), CancellationToken.None));

        Assert.Equal("Credenciales invalidas.", exception.Message);
    }

    [Fact]
    public async Task SocialLogin_ShouldThrowUnauthorized_WhenUserIsInactive()
    {
        var identityService = new FakeIdentityService
        {
            ValidateSocialLoginResult = (false, null, null, Array.Empty<string>(), false, ["El usuario esta inactivo."])
        };
        var handler = new SocialLoginCommandHandler(identityService, new FakeJwtTokenService());

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(new SocialLoginCommand("google", "social-test-token"), CancellationToken.None));

        Assert.Contains("inactivo", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (bool Succeeded, Guid? UserId, IEnumerable<string> Roles, bool IsMember) ValidateCredentialsResult { get; init; }
            = (true, Guid.NewGuid(), ["Member"], false);

        public (bool Succeeded, Guid? UserId, string? Email, IEnumerable<string> Roles, bool IsMember, IEnumerable<string> Errors) ValidateSocialLoginResult { get; init; }
            = (true, Guid.NewGuid(), "social@club.test", ["Member"], false, Array.Empty<string>());

        public Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(string email, string password, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));

        public Task<(bool Succeeded, Guid? UserId, IEnumerable<string> Roles, bool IsMember)> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
            => Task.FromResult(ValidateCredentialsResult);

        public Task<(bool Succeeded, Guid? UserId, string? Email, IEnumerable<string> Roles, bool IsMember, IEnumerable<string> Errors)> ValidateSocialLoginAsync(string provider, string token, CancellationToken cancellationToken)
            => Task.FromResult(ValidateSocialLoginResult);

        public Task<bool> IsUserActiveAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(string email, string password, IEnumerable<string> roles, CancellationToken cancellationToken)
            => Task.FromResult((true, Guid.NewGuid(), Array.Empty<string>() as IEnumerable<string>));

        public Task<IReadOnlyCollection<ManagedUserDto>> ListUsersAsync(string? email, CancellationToken cancellationToken)
            => Task.FromResult((IReadOnlyCollection<ManagedUserDto>)Array.Empty<ManagedUserDto>());

        public Task<(bool Succeeded, IEnumerable<string> Errors)> SetIsMemberAsync(Guid userId, bool isMember, CancellationToken cancellationToken)
            => Task.FromResult((true, Array.Empty<string>() as IEnumerable<string>));

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

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public bool LastIsMember { get; private set; }

        public string CreateToken(Guid userId, string email, IEnumerable<string> roles, bool isMember)
        {
            LastIsMember = isMember;
            return "fake-token";
        }
    }
}

