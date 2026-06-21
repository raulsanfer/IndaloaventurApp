using IndaloAventurApi.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.IntegrationTests;

public sealed class WordPressConfigurationValidationTests
{
    [Fact]
    public void AddInfrastructure_ShouldThrow_WhenWordPressUsernameIsMissing()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:api_ContextConnection"] = "Server=(localdb)\\mssqllocaldb;Database=indalo-test;Trusted_Connection=True;",
                ["Jwt:Issuer"] = "issuer",
                ["Jwt:Audience"] = "audience",
                ["Jwt:Key"] = "key",
                ["SocialAuth:GoogleAudience"] = "google-audience",
                ["WordPress:BaseUrl"] = "https://example.com",
                ["WordPress:Username"] = "",
                ["WordPress:ApplicationPassword"] = "xxxx xxxx xxxx",
                ["WordPress:PostsEndpoint"] = "/wp-json/wp/v2/posts",
                ["WordPress:DefaultPostsPageSize"] = "10",
                ["WordPress:TimeoutSeconds"] = "30"
            })
            .Build();

        var services = new ServiceCollection();
        var exception = Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(config));
        Assert.Contains("WordPress requiere 'Username'", exception.Message);
    }

    [Fact]
    public void AddInfrastructure_ShouldThrow_WhenWordPressDefaultPostsPageSizeIsInvalid()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:api_ContextConnection"] = "Server=(localdb)\\mssqllocaldb;Database=indalo-test;Trusted_Connection=True;",
                ["Jwt:Issuer"] = "issuer",
                ["Jwt:Audience"] = "audience",
                ["Jwt:Key"] = "key",
                ["SocialAuth:GoogleAudience"] = "google-audience",
                ["WordPress:BaseUrl"] = "https://example.com",
                ["WordPress:Username"] = "usuario",
                ["WordPress:ApplicationPassword"] = "xxxx xxxx xxxx",
                ["WordPress:PostsEndpoint"] = "/wp-json/wp/v2/posts",
                ["WordPress:DefaultPostsPageSize"] = "0",
                ["WordPress:TimeoutSeconds"] = "30"
            })
            .Build();

        var services = new ServiceCollection();
        var exception = Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(config));
        Assert.Contains("WordPress requiere 'DefaultPostsPageSize' entre 1 y 100", exception.Message);
    }

    [Fact]
    public void AddInfrastructure_ShouldThrow_WhenSignalImageStorageRootPathIsMissing()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:api_ContextConnection"] = "Server=(localdb)\\mssqllocaldb;Database=indalo-test;Trusted_Connection=True;",
                ["Jwt:Issuer"] = "issuer",
                ["Jwt:Audience"] = "audience",
                ["Jwt:Key"] = "key",
                ["SocialAuth:GoogleAudience"] = "google-audience",
                ["WordPress:BaseUrl"] = "https://example.com",
                ["WordPress:Username"] = "usuario",
                ["WordPress:ApplicationPassword"] = "xxxx xxxx xxxx",
                ["WordPress:PostsEndpoint"] = "/wp-json/wp/v2/posts",
                ["WordPress:DefaultPostsPageSize"] = "10",
                ["WordPress:TimeoutSeconds"] = "30",
                ["SignalImageStorage:RootPath"] = ""
            })
            .Build();

        var services = new ServiceCollection();
        var exception = Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(config));
        Assert.Contains("imagenes de senales requiere 'RootPath'", exception.Message);
    }
}
