using IndaloAventurApi.Infrastructure.Security;

namespace IndaloAventurApi.IntegrationTests;

public sealed class AdminSeedOptionsTests
{
    [Fact]
    public void Defaults_ShouldDisableAdminSeed_AndNotCarryCredentials()
    {
        var options = new AdminSeedOptions();

        Assert.False(options.Enabled);
        Assert.True(string.IsNullOrEmpty(options.Email));
        Assert.True(string.IsNullOrEmpty(options.Password));
    }
}
