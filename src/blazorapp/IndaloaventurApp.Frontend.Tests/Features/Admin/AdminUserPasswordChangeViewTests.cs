namespace IndaloaventurApp.Frontend.Tests.Features.Admin;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class AdminUserPasswordChangeViewTests : BunitContext
{
    /// <summary>
    /// Covers scenario: a valid password change shows the selected user and submits only its request value.
    /// </summary>
    [Fact]
    public void AdminUserPasswordChangeView_SubmitsValidPasswordForManagedUser()
    {
        var userId = Guid.NewGuid();
        AdminUserPasswordChangeRequest? submittedRequest = null;
        var service = CreateService(
            userId,
            "Ana",
            "Montes",
            (_, request, _) =>
            {
                submittedRequest = request;
                return Task.FromResult(ServiceResult<bool>.Success(true));
            });

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminUserPasswordChangeView>(parameters => parameters.Add(x => x.UserId, userId));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Ana Montes", cut.Markup);
            Assert.Single(cut.FindAll("#admin-user-new-password"));
        });

        cut.Find("#admin-user-new-password").Change("Clave123");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(submittedRequest);
            Assert.Equal("Clave123", submittedRequest!.NewPassword);
            Assert.Contains("settings_admin_password_submit_success", cut.Markup);
            Assert.DoesNotContain("Clave123", cut.Markup);
        });
    }

    /// <summary>
    /// Covers scenario: characters outside the allowed alphanumeric range are blocked before submission.
    /// </summary>
    [Fact]
    public void AdminUserPasswordChangeView_RejectsNonAlphanumericPassword()
    {
        var userId = Guid.NewGuid();
        var service = CreateService(userId, "Ana", "Montes");

        Services.AddSingleton<IAdminUserManagementService>(service);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<AdminUserPasswordChangeView>(parameters => parameters.Add(x => x.UserId, userId));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("#admin-user-new-password")));
        cut.Find("#admin-user-new-password").Change("Clave-123");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("solo puede contener caracteres alfanuméricos", cut.Markup, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static RecordingAdminUserManagementService CreateService(
        Guid userId,
        string nombre,
        string apellidos,
        Func<Guid, AdminUserPasswordChangeRequest, CancellationToken, Task<ServiceResult<bool>>>? changePasswordHandler = null)
    {
        var profile = new MemberSelfProfile(
            userId, null, null, nombre, apellidos, "12345678A", new DateOnly(1990, 4, 18),
            "Calle Sierra 5", "04001", "Almeria", "Almeria", "600123123", "ana@club.es", null,
            true, false, false);

        return new RecordingAdminUserManagementService
        {
            GetUserHandler = (_, _) => Task.FromResult(ServiceResult<ManagedUserItem>.Success(
                new ManagedUserItem(userId, "ana@club.es", true, true, new[] { "Member" }))),
            GetMemberFileHandler = (_, _) => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile)),
            ChangeUserPasswordHandler = changePasswordHandler
        };
    }
}
