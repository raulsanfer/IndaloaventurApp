using IndaloAventurApi.Application.Abstractions.Identity;
using MediatR;

namespace IndaloAventurApi.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterCommand, bool>
{
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.RegisterAsync(request.Email, request.Password, cancellationToken);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors));
        }
        return true;
    }
}
