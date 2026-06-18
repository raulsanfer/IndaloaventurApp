using NetArchTest.Rules;

namespace IndaloAventurApi.Architecture.Tests;

public sealed class ArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Projects()
    {
        var result = Types.InAssembly(typeof(IndaloAventurApi.Domain.Abstractions.Entity).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("IndaloAventurApi.Application", "IndaloAventurApi.Infrastructure", "IndaloAventurApi.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Api()
    {
        var result = Types.InAssembly(typeof(IndaloAventurApi.Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn("IndaloAventurApi.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
