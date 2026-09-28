using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Application.Features.Auth.Login;
using MediatR;

namespace IndaloAventurApi.Application.Features.Auth.SocialLogin;

public sealed class SocialLoginCommandHandler(IIdentityService identityService, IJwtTokenService tokenService)
    : IRequestHandler<SocialLoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(SocialLoginCommand request, CancellationToken cancellationToken)
    {
        var validation = await identityService.ValidateSocialLoginAsync(request.Provider, request.Token, cancellationToken);
        if (!validation.Succeeded || validation.UserId is null || string.IsNullOrWhiteSpace(validation.Email))
        {
            throw new UnauthorizedAccessException(string.Join("; ", validation.Errors.DefaultIfEmpty("El token social no es valido.")));
        }

        var token = await tokenService.CreateTokenAsync(validation.UserId.Value, validation.Email, validation.Roles, validation.IsMember, cancellationToken);
        return new LoginResponse(token, "Bearer", 3600, validation.IsMember);
    }
}
