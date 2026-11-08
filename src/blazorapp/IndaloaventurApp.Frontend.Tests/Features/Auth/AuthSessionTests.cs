namespace IndaloaventurApp.Frontend.Tests.Features.Auth;

using IndaloaventurApp.SharedUI.Models.Auth;

public sealed class AuthSessionTests
{
    [Theory]
    [InlineData(false, new[] { "Admin" }, true)]
    [InlineData(true, new[] { "InventoryManager" }, true)]
    [InlineData(false, new[] { "InventoryManager" }, false)]
    [InlineData(true, new[] { "Member" }, false)]
    public void CanManageInventory_AppliesTheDelegatedAccessMatrix(bool isMember, string[] roles, bool expected)
    {
        var session = new AuthSession("access-token", "Bearer", 3600, isMember, roles);

        Assert.Equal(expected, session.CanManageInventory());
    }

    [Fact]
    public void InventoryManagerRole_DoesNotGrantAnotherMemberReservationAccess()
    {
        var session = new AuthSession("access-token", "Bearer", 3600, true, new[] { "InventoryManager" });

        Assert.True(session.CanManageInventory());
        Assert.False(session.CanAccessOwnMaterialReservations());
    }
}
