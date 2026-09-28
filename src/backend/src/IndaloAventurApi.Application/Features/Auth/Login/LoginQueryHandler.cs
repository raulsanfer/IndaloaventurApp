using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Abstractions.Security;
using MediatR;

namespace IndaloAventurApi.Application.Features.Auth.Login;

public sealed class LoginQueryHandler(IIdentityService identityService, IJwtTokenService tokenService)
    : IRequestHandler<LoginQuery, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var validation = await identityService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);
        if (!validation.Succeeded || validation.UserId is null)
        {
            throw new UnauthorizedAccessException("Credenciales invalidas.");
        }

        var token = await tokenService.CreateTokenAsync(validation.UserId.Value, request.Email, validation.Roles, validation.IsMember, cancellationToken);
        return new LoginResponse(token, "Bearer", 3600, validation.IsMember);
    }
}
