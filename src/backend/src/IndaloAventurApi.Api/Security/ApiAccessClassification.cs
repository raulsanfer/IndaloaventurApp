namespace IndaloAventurApi.Api.Security;

public enum ApiAccessClassification
{
    Anonymous,
    Authenticated,
    Admin,
    OwnerOrAdmin,
    ClubMember
}
