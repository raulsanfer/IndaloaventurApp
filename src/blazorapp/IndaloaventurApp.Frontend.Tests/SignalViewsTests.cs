namespace IndaloaventurApp.Frontend.Tests;

using System.Reflection;
using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Components.Signals;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Signals;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class SignalViewsTests : BunitContext
{
    [Fact]
    public void SignalHomeView_RendersDetailLinksForCards()
    {
        var signalService = new RecordingSignalService
        {
            GetSignalHomeDataHandler = (_, _) => Task.FromResult(ServiceResult<SignalHomeData>.Success(
                new SignalHomeData(
                    Array.Empty<SignalCategoryItem>(),
                    new[]
                    {
                        new SignalCardItem(
                            Guid.Parse("7f4b77bd-d4d8-4dfc-8150-a06e6026fdf8"),
                            "Rama caida",
                            "Bloquea el paso",
                            3,
                            "Obstaculo",
                            "bi-sign-turn-right-fill",
                            new DateTime(2026, 6, 8, 8, 30, 0, DateTimeKind.Utc),
                            "sendero",
                            true,
                            null,
                            "sendero",
                            0,
                            0)
                    })))
        };

        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalHomeView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("/signals/7f4b77bd-d4d8-4dfc-8150-a06e6026fdf8", cut.Markup);
            Assert.Contains("signal-home__card-title", cut.Markup);
            Assert.DoesNotContain("signal-home__card-summary", cut.Markup);
            Assert.Contains("signal-home__card-image--placeholder", cut.Markup);
        });
    }

    [Fact]
    public void SignalHomeView_RendersResolvedImage_WhenSignalProvidesImageUrl()
    {
        var signalService = new RecordingSignalService
        {
            GetSignalHomeDataHandler = (_, _) => Task.FromResult(ServiceResult<SignalHomeData>.Success(
                new SignalHomeData(
                    Array.Empty<SignalCategoryItem>(),
                    new[]
                    {
                        new SignalCardItem(
                            Guid.Parse("7f4b77bd-d4d8-4dfc-8150-a06e6026fdf8"),
                            "Rama caida",
                            "Bloquea el paso",
                            3,
                            "Obstaculo",
                            "bi-sign-turn-right-fill",
                            new DateTime(2026, 6, 8, 8, 30, 0, DateTimeKind.Utc),
                            "sendero",
                            true,
                            "data:image/jpeg;base64,/9j/",
                            "sendero",
                            0,
                            0)
                    })))
        };

        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalHomeView>();

        cut.WaitForAssertion(() =>
        {
            var image = cut.Find(".signal-home__card-image");
            Assert.Equal("data:image/jpeg;base64,/9j/", image.GetAttribute("src"));
            Assert.DoesNotContain("signal-home__card-image--placeholder", cut.Markup);
        });
    }

    [Fact]
    public void SignalDetailView_ShowsLocationEmptyState_WhenSignalHasNoCoordinates()
    {
        var signalService = new RecordingSignalService
        {
            GetSignalHandler = (id, _) => Task.FromResult(ServiceResult<SignalDetailItem>.Success(
                BuildDetailItem(
                    id,
                    title: "Rama caida",
                    description: "Bloquea el paso junto al cortijo.",
                    latitude: 0,
                    longitude: 0,
                    ownerUserId: Guid.NewGuid()))),
            GetSignalCommentsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(Array.Empty<SignalCommentItem>()))
        };

        RegisterDetailServices(signalService);

        var cut = Render<SignalDetailView>(parameters => parameters.Add(x => x.SignalId, Guid.NewGuid()));

        cut.WaitForAssertion(() => Assert.Contains("signal_detail_tab_map", cut.Markup));
        cut.FindAll("button[role='tab']")[1].Click();
        cut.WaitForAssertion(() => Assert.Contains("signal_detail_location_empty", cut.Markup));
    }

    [Fact]
    public void SignalDetailView_PreservesBreadcrumbAndRendersOverviewHeader()
    {
        var signalId = Guid.NewGuid();
        var signalService = new RecordingSignalService
        {
            GetSignalHandler = (id, _) => Task.FromResult(ServiceResult<SignalDetailItem>.Success(
                BuildDetailItem(
                    id,
                    title: "Piedra desprendida",
                    description: "Hay una piedra de gran tamano invadiendo parte del sendero.",
                    categoryId: 8,
                    categoryName: "Senderismo",
                    tags: "senderos, piedras, mantenimiento",
                    ownerUserId: Guid.NewGuid()))),
            GetSignalCommentsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(Array.Empty<SignalCommentItem>()))
        };

        RegisterDetailServices(signalService);

        var cut = Render<SignalDetailView>(parameters => parameters.Add(x => x.SignalId, signalId));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("signal_detail_back", cut.Markup);
            Assert.Contains("Piedra desprendida", cut.Markup);
            Assert.Contains("Senderismo", cut.Markup);
            Assert.Contains("signal_detail_field_created", cut.Markup);
            Assert.Contains("signal_detail_field_updated", cut.Markup);
        });
    }

    [Fact]
    public void SignalDetailView_ShowsMap_WhenSignalHasCoordinates()
    {
        var signalService = new RecordingSignalService
        {
            GetSignalHandler = (id, _) => Task.FromResult(ServiceResult<SignalDetailItem>.Success(
                BuildDetailItem(
                    id,
                    title: "Rama caida",
                    description: "Bloquea el paso junto al cortijo.",
                    ownerUserId: Guid.NewGuid()))),
            GetSignalCommentsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(Array.Empty<SignalCommentItem>()))
        };

        RegisterDetailServices(signalService);

        var cut = Render<SignalDetailView>(parameters => parameters.Add(x => x.SignalId, Guid.NewGuid()));

        cut.WaitForAssertion(() => Assert.Contains("signal_detail_tab_map", cut.Markup));
        cut.FindAll("button[role='tab']")[1].Click();
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("openstreetmap.org/export/embed.html", cut.Markup);
            Assert.Contains("signal_detail_map_open", cut.Markup);
        });
    }

    [Fact]
    public void SignalDetailView_RendersCommentsBelowTabsAndTagsAtEnd()
    {
        var signalService = new RecordingSignalService
        {
            GetSignalHandler = (id, _) => Task.FromResult(ServiceResult<SignalDetailItem>.Success(
                BuildDetailItem(
                    id,
                    title: "Piedra desprendida",
                    description: "Hay una piedra de gran tamano invadiendo parte del sendero.",
                    categoryId: 8,
                    categoryName: "Senderismo",
                    tags: "senderos, piedras, mantenimiento",
                    ownerUserId: Guid.NewGuid()))),
            GetSignalCommentsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(
                new[]
                {
                    new SignalCommentItem(
                        Guid.NewGuid(),
                        new DateTime(2026, 6, 4, 9, 45, 0, DateTimeKind.Utc),
                        "La incidencia sigue presente y ocupa media calzada.")
                }))
        };

        RegisterDetailServices(signalService);

        var cut = Render<SignalDetailView>(parameters => parameters.Add(x => x.SignalId, Guid.NewGuid()));

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(2, cut.FindAll("button[role='tab']").Count);
            Assert.Contains("La incidencia sigue presente y ocupa media calzada.", cut.Markup);
            Assert.Contains("signal_detail_comments_count", cut.Markup);
            Assert.Contains("signal_detail_tags_count", cut.Markup);
            Assert.True(cut.Markup.IndexOf("La incidencia sigue presente y ocupa media calzada.", StringComparison.Ordinal) <
                        cut.Markup.IndexOf("senderos", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void SignalDetailView_ShowsEditButton_ForSignalOwner()
    {
        var signalId = Guid.NewGuid();
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var signalService = new RecordingSignalService
        {
            GetSignalHandler = (id, _) => Task.FromResult(ServiceResult<SignalDetailItem>.Success(
                BuildDetailItem(
                    id,
                    title: "Piedra desprendida",
                    description: "Hay una piedra de gran tamano invadiendo parte del sendero.",
                    categoryId: 8,
                    categoryName: "Senderismo",
                    tags: "senderos, piedras, mantenimiento",
                    ownerUserId: TestSessions.MemberSession.UserId!.Value))),
            GetSignalCommentsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(Array.Empty<SignalCommentItem>()))
        };

        RegisterDetailServices(signalService, sessionService);

        var cut = Render<SignalDetailView>(parameters => parameters.Add(x => x.SignalId, signalId));

        cut.WaitForAssertion(() => Assert.Contains("signal_detail_edit_action", cut.Markup));
    }

    [Fact]
    public void SignalDetailView_HidesEditButton_ForDifferentUser()
    {
        var signalId = Guid.NewGuid();
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.AdminSession);

        var signalService = new RecordingSignalService
        {
            GetSignalHandler = (id, _) => Task.FromResult(ServiceResult<SignalDetailItem>.Success(
                BuildDetailItem(
                    id,
                    title: "Piedra desprendida",
                    description: "Hay una piedra de gran tamano invadiendo parte del sendero.",
                    categoryId: 8,
                    categoryName: "Senderismo",
                    tags: "senderos, piedras, mantenimiento",
                    ownerUserId: TestSessions.MemberSession.UserId!.Value))),
            GetSignalCommentsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(Array.Empty<SignalCommentItem>()))
        };

        RegisterDetailServices(signalService, sessionService);

        var cut = Render<SignalDetailView>(parameters => parameters.Add(x => x.SignalId, signalId));

        cut.WaitForAssertion(() => Assert.DoesNotContain("signal_detail_edit_action", cut.Markup));
    }

    [Fact]
    public void SignalDetailView_SavesOwnSignalEdit()
    {
        var signalId = Guid.NewGuid();
        var loadCount = 0;
        UpdateSignalRequest? capturedRequest = null;
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var signalService = new RecordingSignalService
        {
            GetSignalHandler = (id, _) =>
            {
                loadCount++;
                return Task.FromResult(ServiceResult<SignalDetailItem>.Success(
                    BuildDetailItem(
                        id,
                        title: loadCount == 1 ? "Titulo inicial" : "Titulo actualizado",
                        description: loadCount == 1 ? "Descripcion inicial" : "Descripcion actualizada",
                        isActive: loadCount == 1,
                        ownerUserId: TestSessions.MemberSession.UserId!.Value)));
            },
            GetSignalCommentsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(Array.Empty<SignalCommentItem>())),
            UpdateSignalHandler = (request, _) =>
            {
                capturedRequest = request;
                return Task.FromResult(ServiceResult<bool>.Success(true));
            }
        };

        RegisterDetailServices(signalService, sessionService);

        var cut = Render<SignalDetailView>(parameters => parameters.Add(x => x.SignalId, signalId));

        cut.WaitForAssertion(() => Assert.Contains("signal_detail_edit_action", cut.Markup));
        cut.Find(".signal-detail__edit-trigger").Click();
        cut.Find("#signal-edit-title").Change("Titulo actualizado");
        cut.Find("#signal-edit-description").Change("Descripcion actualizada");
        cut.Find("#signal-edit-status").Change("false");
        cut.Find(".signal-detail__edit-actions .btn.btn-primary").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(capturedRequest);
            Assert.Equal(signalId, capturedRequest!.SignalId);
            Assert.Equal("Titulo actualizado", capturedRequest.Title);
            Assert.Equal("Descripcion actualizada", capturedRequest.Description);
            Assert.False(capturedRequest.IsActive);
            Assert.Contains("signal_detail_edit_success", cut.Markup);
            Assert.Contains("Titulo actualizado", cut.Markup);
        });
    }

    [Fact]
    public void SignalDetailView_KeepsDraft_WhenSaveFails()
    {
        var signalId = Guid.NewGuid();
        var sessionService = new RecordingSessionService();
        sessionService.SetSession(TestSessions.MemberSession);

        var signalService = new RecordingSignalService
        {
            GetSignalHandler = (id, _) => Task.FromResult(ServiceResult<SignalDetailItem>.Success(
                BuildDetailItem(
                    id,
                    title: "Titulo inicial",
                    description: "Descripcion inicial",
                    ownerUserId: TestSessions.MemberSession.UserId!.Value))),
            GetSignalCommentsHandler = (_, _) => Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(Array.Empty<SignalCommentItem>())),
            UpdateSignalHandler = (_, _) => Task.FromResult(ServiceResult<bool>.Failure(new ServiceError("signals.update_forbidden", "forbidden")))
        };

        RegisterDetailServices(signalService, sessionService);

        var cut = Render<SignalDetailView>(parameters => parameters.Add(x => x.SignalId, signalId));

        cut.WaitForAssertion(() => Assert.Contains("signal_detail_edit_action", cut.Markup));
        cut.Find(".signal-detail__edit-trigger").Click();
        cut.Find("#signal-edit-title").Change("Titulo con error");
        cut.Find(".signal-detail__edit-actions .btn.btn-primary").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("signal_detail_edit_forbidden", cut.Markup);
            Assert.Equal("Titulo con error", cut.Find("#signal-edit-title").GetAttribute("value"));
        });
    }

    [Fact]
    public void SignalCreateView_RendersSeparateCameraAndGalleryInputsPerPhotoSlot()
    {
        var signalService = BuildSignalServiceForCreate();
        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalCreateView>();

        NavigateToPhotosStep(cut);

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("signal-photo-1-camera-input", cut.Markup);
            Assert.Contains("signal-photo-1-gallery-input", cut.Markup);
            Assert.Contains("signal-photo-2-camera-input", cut.Markup);
            Assert.Contains("signal-photo-2-gallery-input", cut.Markup);
            Assert.Equal("environment", cut.Find("#signal-photo-1-camera-input").GetAttribute("capture"));
            Assert.Equal("environment", cut.Find("#signal-photo-2-camera-input").GetAttribute("capture"));
            Assert.Null(cut.Find("#signal-photo-1-gallery-input").GetAttribute("capture"));
            Assert.Null(cut.Find("#signal-photo-2-gallery-input").GetAttribute("capture"));
            Assert.Contains("signal_create_photo_1_camera_button", cut.Markup);
            Assert.Contains("signal_create_photo_1_gallery_button", cut.Markup);
        });
    }

    [Fact]
    public void SignalCreateView_UsesDistinctInteropTargetsForCameraAndGalleryButtons()
    {
        var signalService = BuildSignalServiceForCreate();
        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
        JSInterop.SetupVoid("indaloSignals.triggerFileInput", _ => true).SetVoidResult();

        var cut = Render<SignalCreateView>();

        NavigateToPhotosStep(cut);

        cut.FindAll(".signal-create__photo-actions .btn")[0].Click();
        cut.FindAll(".signal-create__photo-actions .btn")[1].Click();

        cut.WaitForAssertion(() =>
        {
            var invocations = JSInterop.Invocations
                .Where(invocation => invocation.Identifier == "indaloSignals.triggerFileInput")
                .ToArray();

            Assert.Equal(2, invocations.Length);
            Assert.Equal("signal-photo-1-camera-input", invocations[0].Arguments[0]?.ToString());
            Assert.Equal("signal-photo-1-gallery-input", invocations[1].Arguments[0]?.ToString());
        });
    }

    [Fact]
    public void SignalCreateView_AllowsMovingToReviewWithOnlyPhoto1()
    {
        var signalService = BuildSignalServiceForCreate();
        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalCreateView>();

        NavigateToPhotosStep(cut);
        SetDraftPhotos(cut, CreatePhoto("foto-1"), null);

        cut.Find(".signal-create__actions .btn.btn-primary").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("signal_create_review_title", cut.Markup);
            Assert.DoesNotContain("signal_create_photos_required", cut.Markup);
        });
    }

    [Fact]
    public void SignalCreateView_RemovesPhoto2FromReviewWithoutAffectingPhoto1()
    {
        var signalService = BuildSignalServiceForCreate();
        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalCreateView>();

        PrepareReviewStep(cut, CreatePhoto("foto-1"), CreatePhoto("foto-2"));

        cut.Find(".signal-create__review-photo-remove").Click();

        cut.WaitForAssertion(() =>
        {
            var draft = GetDraft(cut);
            Assert.NotNull(draft.Photo1);
            Assert.Null(draft.Photo2);
            Assert.Single(cut.FindAll(".signal-create__review-photo img"));
            Assert.DoesNotContain("signal_create_photo_2_remove", cut.Markup);
        });
    }

    [Fact]
    public void SignalCreateView_SendsEmptyPhoto2_WhenSavingWithOnlyPhoto1()
    {
        SignalCreateRequest? capturedRequest = null;
        var signalService = new RecordingSignalService
        {
            GetSignalCategoriesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<SignalCategoryItem>>.Success(
                new[]
                {
                    new SignalCategoryItem(3, "Obstaculo", "bi-sign-turn-right-fill")
                })),
            CreateSignalHandler = (request, _) =>
            {
                capturedRequest = request;
                return Task.FromResult(ServiceResult<Guid>.Success(Guid.NewGuid()));
            }
        };

        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<SignalCreateView>();

        PrepareReviewStep(cut, CreatePhoto("foto-1"), null);

        cut.Find(".signal-create__actions .btn.btn-primary").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(capturedRequest);
            Assert.Equal("Rama caida en sendero", capturedRequest!.Title);
            Assert.Equal("Rama caida en la entrada del sendero.", capturedRequest.Description);
            Assert.Equal("sendero,seguridad", capturedRequest.Tags);
            Assert.Equal("foto-1"u8.ToArray(), capturedRequest.Photo1);
            Assert.Empty(capturedRequest.Photo2);
            Assert.Contains("signal_create_success_title", cut.Markup);
        });
    }

    private void RegisterDetailServices(ISignalService signalService, ISessionService? sessionService = null)
    {
        Services.AddSingleton<ISignalService>(signalService);
        Services.AddSingleton<ISessionService>(sessionService ?? new RecordingSessionService());
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
    }

    private static SignalDetailItem BuildDetailItem(
        Guid id,
        string title,
        string description,
        int categoryId = 3,
        string categoryName = "Obstaculo",
        string? tags = "sendero, rama",
        bool isActive = true,
        float latitude = 36.834652f,
        float longitude = -2.463714f,
        Guid? ownerUserId = null)
    {
        return new SignalDetailItem(
            id,
            title,
            description,
            categoryId,
            categoryName,
            "bi-sign-turn-right-fill",
            isActive,
            new DateTime(2026, 6, 7, 8, 15, 0, DateTimeKind.Utc),
            new DateTime(2026, 6, 8, 10, 30, 0, DateTimeKind.Utc),
            tags,
            latitude,
            longitude,
            ownerUserId ?? Guid.NewGuid());
    }

    private static RecordingSignalService BuildSignalServiceForCreate()
    {
        return new RecordingSignalService
        {
            GetSignalCategoriesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<SignalCategoryItem>>.Success(
                new[]
                {
                    new SignalCategoryItem(3, "Obstaculo", "bi-sign-turn-right-fill")
                }))
        };
    }

    private static void NavigateToPhotosStep(IRenderedComponent<SignalCreateView> cut)
    {
        cut.WaitForAssertion(() => Assert.Contains("signal_create_type_title", cut.Markup));
        cut.Find(".signal-create__type-option").Click();
        cut.Find("#signal-title").Change("Rama caida en sendero");
        cut.Find("#signal-description").Change("Rama caida en la entrada del sendero.");
        cut.Find("#signal-tags").Change("sendero, seguridad");
        cut.Find(".signal-create__actions .btn.btn-primary").Click();
        cut.WaitForAssertion(() => Assert.Contains("signal_create_photos_title", cut.Markup));
    }

    private static void PrepareReviewStep(
        IRenderedComponent<SignalCreateView> cut,
        SignalPhotoDraft photo1,
        SignalPhotoDraft? photo2)
    {
        NavigateToPhotosStep(cut);
        SetDraftPhotos(cut, photo1, photo2);
        SetCurrentStep(cut, "Review");
        cut.Render();
        cut.WaitForAssertion(() => Assert.Contains("signal_create_review_title", cut.Markup));
    }

    private static void SetDraftPhotos(
        IRenderedComponent<SignalCreateView> cut,
        SignalPhotoDraft photo1,
        SignalPhotoDraft? photo2)
    {
        var draft = GetDraft(cut);
        draft.Photo1 = photo1;
        draft.Photo2 = photo2;
        cut.Render();
    }

    private static SignalCreateDraft GetDraft(IRenderedComponent<SignalCreateView> cut)
    {
        return (SignalCreateDraft)typeof(SignalCreateView)
            .GetProperty("Draft", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .GetValue(cut.Instance)!;
    }

    private static void SetCurrentStep(IRenderedComponent<SignalCreateView> cut, string stepName)
    {
        var currentStepProperty = typeof(SignalCreateView)
            .GetProperty("CurrentStep", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        var stepValue = Enum.Parse(currentStepProperty.PropertyType, stepName);
        currentStepProperty.SetValue(cut.Instance, stepValue);
    }

    private static SignalPhotoDraft CreatePhoto(string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return new SignalPhotoDraft(bytes, "image/jpeg", $"{content}.jpg", $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}");
    }
}
