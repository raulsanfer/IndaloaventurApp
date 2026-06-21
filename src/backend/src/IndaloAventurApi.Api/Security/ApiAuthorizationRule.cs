namespace IndaloAventurApi.Api.Security;

public sealed record ApiAuthorizationRule(
    string HttpMethod,
    string Route,
    ApiAccessClassification Classification,
    string Enforcement,
    string Notes);
