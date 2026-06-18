namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Components.MyAccount;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class MemberSelfProfileViewTests : BunitContext
{
    [Fact]
    public void MemberSelfProfileView_ShowsNonOperationalState_ForNonMember()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(new IndaloaventurApp.SharedUI.Models.Auth.AuthSession("token", "Bearer", 3600, false));

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IMemberProfileService>(new RecordingMemberProfileService());
        Services.AddSingleton<ICargoAdminService>(new RecordingCargoAdminService());
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<MemberSelfProfileView>();

        Assert.Contains("member_file_not_available", cut.Markup);
        Assert.DoesNotContain("member_file_save", cut.Markup);
    }

    [Fact]
    public void MemberSelfProfileView_RendersFormAndSubmitsSanitizedPayload()
    {
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        UpdateMemberSelfProfileRequest? submittedRequest = null;
        var profile = new MemberSelfProfile(
            Guid.NewGuid(),
            7,
            "Presidencia",
            "Ana",
            "Montes",
            "12345678A",
            new DateOnly(1990, 4, 18),
            "Calle Sierra 5",
            "04001",
            "Almería",
            "Almería",
            "600123123",
            "ana@club.es",
            "Polen",
            true,
            false,
            true);

        var service = new RecordingMemberProfileService
        {
            GetMyMemberFileHandler = _ => Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile)),
            UpdateMyMemberFileHandler = (request, _) =>
            {
                submittedRequest = request;
                return Task.FromResult(ServiceResult<MemberSelfProfile>.Success(profile with
                {
                    Nombre = request.Nombre,
                    Email = request.Email
                }));
            }
        };

        Services.AddSingleton<ISessionService>(sessionService);
        Services.AddSingleton<IMemberProfileService>(service);
        Services.AddSingleton<ICargoAdminService>(new RecordingCargoAdminService
        {
            GetCargosHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(
                new[]
                {
                    new CargoItem(7, "Presidencia")
                }))
        });
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<MemberSelfProfileView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("member_file_title", cut.Markup);
            Assert.Contains("member_file_consents_section", cut.Markup);
            Assert.Contains("value=\"Presidencia\"", cut.Markup);
            Assert.Empty(cut.FindAll("select#member-cargo-id"));
            Assert.DoesNotContain("IsMember", cut.Markup);
            Assert.DoesNotContain("roles", cut.Markup, StringComparison.OrdinalIgnoreCase);
        });

        cut.Find("#member-nombre").Change("  Ana  ");
        cut.Find("#member-email").Change("ANA@CLUB.ES");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(submittedRequest);
            Assert.Equal("Ana", submittedRequest!.Nombre);
            Assert.Equal("ana@club.es", submittedRequest.Email);
            Assert.Contains("member_file_save_success", cut.Markup);
        });
    }
}
