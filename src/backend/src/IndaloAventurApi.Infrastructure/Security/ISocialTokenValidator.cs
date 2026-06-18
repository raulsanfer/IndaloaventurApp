namespace IndaloAventurApi.Infrastructure.Security;

public interface ISocialTokenValidator
{
    Task<SocialTokenValidationResult> ValidateAsync(string provider, string token, CancellationToken cancellationToken);
}
