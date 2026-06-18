namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Signals;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class SignalCategoriesManagementViewTests : BunitContext
{
    [Fact]
    public void SignalSettingsHubView_RendersCategoriesEntry()
    {
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalSettingsHubView>();

        Assert.Contains("settings_admin_signals_title", cut.Markup);
        Assert.Contains("/configuracion/signals/categorias", cut.Markup);
        Assert.Contains("settings_admin_signal_categories_title", cut.Markup);
    }

    [Fact]
    public void SignalCategoriesManagementView_RendersListAndEmptyEditor()
    {
        var signalService = new RecordingSignalService
        {
            GetSignalCategoriesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<SignalCategoryItem>>.Success(
                new[]
                {
                    new SignalCategoryItem(1, "Obstáculo", "bi-sign-turn-right-fill"),
                    new SignalCategoryItem(2, "Seguridad", null)
                }))
        };

        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalCategoriesManagementView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("settings_signal_categories_title", cut.Markup);
            Assert.Contains("Obstáculo", cut.Markup);
            Assert.Contains("Seguridad", cut.Markup);
            Assert.Contains("settings_signal_categories_new_title", cut.Markup);
            Assert.Contains("signal-category-name", cut.Markup);
            Assert.Contains("signal-category-icon", cut.Markup);
        });
    }

    [Fact]
    public void SignalCategoriesManagementView_CreatesCategoryAndRefreshesList()
    {
        var categories = new List<SignalCategoryItem>
        {
            new(1, "Obstáculo", "bi-sign-turn-right-fill")
        };

        CreateSignalCategoryRequest? createRequest = null;

        var signalService = new RecordingSignalService
        {
            GetSignalCategoriesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<SignalCategoryItem>>.Success(categories.ToArray())),
            CreateSignalCategoryHandler = (request, _) =>
            {
                createRequest = request;
                categories.Add(new SignalCategoryItem(2, request.Name, request.IconName));
                return Task.FromResult(ServiceResult<int>.Success(2));
            }
        };

        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalCategoriesManagementView>();

        cut.WaitForAssertion(() => Assert.Contains("Obstáculo", cut.Markup));

        cut.Find("#signal-category-name").Change("Seguridad");
        cut.Find("#signal-category-icon").Change("shield-fill");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(createRequest);
            Assert.Equal("Seguridad", createRequest!.Name);
            Assert.Equal("bi-shield-fill", createRequest.IconName);
            Assert.Contains("settings_signal_categories_create_success", cut.Markup);
            Assert.Contains("Seguridad", cut.Markup);
            Assert.DoesNotContain("value=\"Seguridad\"", cut.Find("#signal-category-name").OuterHtml);
        });
    }

    [Fact]
    public void SignalCategoriesManagementView_UpdatesCategoryAfterSelectingEdit()
    {
        var categories = new List<SignalCategoryItem>
        {
            new(1, "Obstáculo", "bi-sign-turn-right-fill")
        };

        UpdateSignalCategoryRequest? updateRequest = null;

        var signalService = new RecordingSignalService
        {
            GetSignalCategoriesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<SignalCategoryItem>>.Success(categories.ToArray())),
            UpdateSignalCategoryHandler = (request, _) =>
            {
                updateRequest = request;
                categories[0] = new SignalCategoryItem(request.Id, request.Name, request.IconName);
                return Task.FromResult(ServiceResult<bool>.Success(true));
            }
        };

        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalCategoriesManagementView>();

        cut.WaitForAssertion(() => Assert.Contains("Obstáculo", cut.Markup));

        cut.FindAll("button")
            .First(button => button.TextContent.Contains("settings_signal_categories_edit", StringComparison.Ordinal))
            .Click();

        cut.WaitForAssertion(() => Assert.Equal("Obstáculo", cut.Find("#signal-category-name").GetAttribute("value")));

        cut.Find("#signal-category-name").Change("Obstáculo temporal");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(updateRequest);
            Assert.Equal(1, updateRequest!.Id);
            Assert.Equal("Obstáculo temporal", updateRequest.Name);
            Assert.Contains("settings_signal_categories_update_success", cut.Markup);
            Assert.Contains("Obstáculo temporal", cut.Markup);
        });
    }

    [Fact]
    public void SignalCategoriesManagementView_ShowsBlockedDeleteMessage_WhenBackendRejectsDelete()
    {
        var categories = new List<SignalCategoryItem>
        {
            new(1, "Obstáculo", "bi-sign-turn-right-fill")
        };

        var signalService = new RecordingSignalService
        {
            GetSignalCategoriesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<SignalCategoryItem>>.Success(categories.ToArray())),
            DeleteSignalCategoryHandler = (_, _) => Task.FromResult(ServiceResult<bool>.Failure(new ServiceError("signals.categories.delete_blocked", "blocked")))
        };

        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalCategoriesManagementView>();

        cut.WaitForAssertion(() => Assert.Contains("Obstáculo", cut.Markup));

        cut.FindAll("button")
            .First(button => button.TextContent.Contains("settings_signal_categories_delete", StringComparison.Ordinal))
            .Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("settings_signal_categories_delete_blocked", cut.Markup);
            Assert.Contains("Obstáculo", cut.Markup);
        });
    }
}
