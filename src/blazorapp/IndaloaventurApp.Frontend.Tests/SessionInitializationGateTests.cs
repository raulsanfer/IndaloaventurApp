namespace IndaloaventurApp.Frontend.Tests;

using System;
using System.Threading.Tasks;
using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.Web.Client;
using Microsoft.Extensions.DependencyInjection;

public sealed class SessionInitializationGateTests : BunitContext
{
    [Fact]
    public async Task HidesChildContentUntilSessionInitializationCompletes()
    {
        var initializationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var completionTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var sessionService = new RecordingSessionService
        {
            IsInitialized = false
        };

        sessionService.EnsureInitializedHandler = async () =>
        {
            await initializationTcs.Task;
            sessionService.IsInitialized = true;
            completionTcs.SetResult();
        };

        Services.AddSingleton<ISessionService>(sessionService);

        var cut = Render<SessionInitializationGate>(parameters => parameters
            .AddChildContent("<div id=\"protected-content\">ready</div>"));

        Assert.Contains("session-initialization-gate", cut.Markup);
        Assert.DoesNotContain("protected-content", cut.Markup);

        initializationTcs.SetResult();

        await completionTcs.Task.WaitAsync(TimeSpan.FromSeconds(15));
        cut.Render();

        Assert.Equal(1, sessionService.EnsureInitializedCallCount);
        Assert.Contains("protected-content", cut.Markup);
    }
}
