namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class CargoManagementViewTests : BunitContext
{
    [Fact]
    public void CargoManagementView_RendersBreadcrumbFieldsetAndList()
    {
        var cargoService = new RecordingCargoAdminService
        {
            GetCargosHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(
                new[]
                {
                    new CargoItem(1, "Presidencia"),
                    new CargoItem(2, "Secretaría")
                }))
        };

        Services.AddSingleton<ICargoAdminService>(cargoService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<CargoManagementView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("settings_cargos_breadcrumb_label", cut.Markup);
            Assert.Contains("settings_cargos_new_title", cut.Markup);
            Assert.Contains("settings_admin_cargos_list_title", cut.Markup);
            Assert.Contains("settings_cargos_editor_summary", cut.Markup);
            Assert.Contains("settings_cargos_list_summary", cut.Markup);
            Assert.Contains("class=\"input input-bordered", cut.Markup);
            Assert.Contains("class=\"btn btn-primary", cut.Markup);
            Assert.Contains("settings-cargos__list-section", cut.Markup);
            Assert.Contains("Presidencia", cut.Markup);
            Assert.Contains("Secretaría", cut.Markup);
            Assert.Contains("settings_cargos_delete_coming_soon", cut.Markup);
        });
    }

    [Fact]
    public void CargoManagementView_CreatesCargoAndResetsToNewMode()
    {
        var cargos = new List<CargoItem>
        {
            new(1, "Presidencia")
        };

        CreateCargoRequest? createRequest = null;
        var getCalls = 0;

        var cargoService = new RecordingCargoAdminService
        {
            GetCargosHandler = _ =>
            {
                getCalls++;
                return Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(cargos.ToArray()));
            },
            CreateCargoHandler = (request, _) =>
            {
                createRequest = request;
                cargos.Add(new CargoItem(2, request.Description));
                return Task.FromResult(ServiceResult<CargoItem>.Success(new CargoItem(2, request.Description)));
            }
        };

        Services.AddSingleton<ICargoAdminService>(cargoService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<CargoManagementView>();

        cut.WaitForAssertion(() => Assert.Contains("Presidencia", cut.Markup));

        cut.Find("#cargo-description").Change("Tesorería");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(createRequest);
            Assert.Equal("Tesorería", createRequest!.Description);
            Assert.Contains("settings_cargos_create_success", cut.Markup);
            Assert.Contains("settings_cargos_new_title", cut.Markup);
            Assert.Contains("Tesorería", cut.Markup);
            Assert.True(getCalls >= 2);
            Assert.DoesNotContain("value=\"Tesorería\"", cut.Find("#cargo-description").OuterHtml);
        });
    }

    [Fact]
    public void CargoManagementView_LoadsSelectedCargoIntoFieldsetAndUpdatesIt()
    {
        var cargos = new List<CargoItem>
        {
            new(1, "Presidencia"),
            new(2, "Secretaría")
        };

        UpdateCargoRequest? updateRequest = null;

        var cargoService = new RecordingCargoAdminService
        {
            GetCargosHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(cargos.ToArray())),
            UpdateCargoHandler = (request, _) =>
            {
                updateRequest = request;
                var cargo = cargos.Single(x => x.Id == request.Id);
                cargos[cargos.IndexOf(cargo)] = cargo with { Description = request.Description };
                return Task.FromResult(ServiceResult<CargoItem>.Success(new CargoItem(request.Id, request.Description)));
            }
        };

        Services.AddSingleton<ICargoAdminService>(cargoService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<CargoManagementView>();

        cut.WaitForAssertion(() => Assert.Contains("Secretaría", cut.Markup));

        cut.FindAll("button")
            .First(button => button.TextContent.Contains("settings_cargos_edit", StringComparison.Ordinal))
            .Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("settings_cargos_edit_title", cut.Markup);
            Assert.Equal("Presidencia", cut.Find("#cargo-description").GetAttribute("value"));
        });

        cut.Find("#cargo-description").Change("Presidencia honorífica");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(updateRequest);
            Assert.Equal(1, updateRequest!.Id);
            Assert.Equal("Presidencia honorífica", updateRequest.Description);
            Assert.Contains("settings_cargos_update_success", cut.Markup);
            Assert.Contains("Presidencia honorífica", cut.Markup);
            Assert.Contains("settings_cargos_new_title", cut.Markup);
        });
    }
}
