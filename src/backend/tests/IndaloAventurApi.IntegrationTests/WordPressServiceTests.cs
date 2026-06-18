using System.Net;
using System.Text;
using IndaloAventurApi.Application.Abstractions.WordPress;
using IndaloAventurApi.Infrastructure.WordPress;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.IntegrationTests;

public sealed class WordPressServiceTests
{
    [Fact]
    public async Task GetPostsAsync_ShouldMapPosts_WhenResponseIsSuccessful()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(
                "/wp-json/wp/v2/posts?page=1&per_page=10&status=publish&orderby=date&order=desc&_embed=wp:featuredmedia&_fields=id,slug,date_gmt,title.rendered,excerpt.rendered,_links,_embedded",
                request.RequestUri!.PathAndQuery);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
[
  {
    "id": 10,
    "slug": "post-prueba",
    "date_gmt": "2026-05-24T10:00:00Z",
    "title": { "rendered": "Titulo prueba" },
    "excerpt": { "rendered": "Resumen prueba" },
    "_embedded": {
      "wp:featuredmedia": [
        { "source_url": "https://example.com/images/post-prueba.jpg" }
      ]
    }
  }
]
""", Encoding.UTF8, "application/json")
            };
        });

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com"),
            Timeout = TimeSpan.FromSeconds(5)
        };
        var options = Options.Create(new WordPressOptions
        {
            BaseUrl = "https://example.com",
            Username = "usuario",
            ApplicationPassword = "xxxx xxxx xxxx",
            PostsEndpoint = "/wp-json/wp/v2/posts",
            DefaultPostsPageSize = 10,
            TimeoutSeconds = 5
        });

        var service = new WordPressService(client, options);
        var posts = await service.GetPostsAsync(1, 10, null, CancellationToken.None);

        var post = Assert.Single(posts);
        Assert.Equal(10, post.Id);
        Assert.Equal("post-prueba", post.Slug);
        Assert.Equal("Titulo prueba", post.Titulo);
        Assert.Equal("Resumen prueba", post.Resumen);
        Assert.Equal("https://example.com/images/post-prueba.jpg", post.ImagenDestacadaUrl);
    }

    [Fact]
    public async Task GetPostsAsync_ShouldThrowControlledError_WhenRemoteReturnsError()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadGateway));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var options = Options.Create(new WordPressOptions
        {
            BaseUrl = "https://example.com",
            Username = "usuario",
            ApplicationPassword = "xxxx xxxx xxxx",
            PostsEndpoint = "/wp-json/wp/v2/posts",
            DefaultPostsPageSize = 10,
            TimeoutSeconds = 5
        });

        var service = new WordPressService(client, options);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetPostsAsync(1, 10, null, CancellationToken.None));

        Assert.Contains("WordPress devolvio un error remoto", exception.Message);
    }

    [Fact]
    public async Task GetPostsAsync_ShouldThrowControlledError_WhenConnectionFails()
    {
        var handler = new StubHttpMessageHandler(_ => throw new HttpRequestException("network"));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var options = Options.Create(new WordPressOptions
        {
            BaseUrl = "https://example.com",
            Username = "usuario",
            ApplicationPassword = "xxxx xxxx xxxx",
            PostsEndpoint = "/wp-json/wp/v2/posts",
            DefaultPostsPageSize = 10,
            TimeoutSeconds = 5
        });

        var service = new WordPressService(client, options);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetPostsAsync(1, 10, null, CancellationToken.None));

        Assert.Equal("No se pudo conectar con WordPress.", exception.Message);
    }

    [Fact]
    public async Task GetPostBySlugAsync_ShouldMapPost_WhenSlugExists()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal("/wp-json/wp/v2/posts?slug=post-prueba&_embed=true", request.RequestUri!.PathAndQuery);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
[
  {
    "id": 10,
    "slug": "post-prueba",
    "link": "https://example.com/post-prueba",
    "date_gmt": "2026-05-24T10:00:00Z",
    "title": { "rendered": "Titulo prueba" },
    "excerpt": { "rendered": "Resumen prueba" },
    "content": { "rendered": "Contenido completo prueba" },
    "_embedded": {
      "wp:featuredmedia": [
        { "source_url": "https://example.com/images/post-prueba.jpg" }
      ]
    }
  }
]
""", Encoding.UTF8, "application/json")
            };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var options = Options.Create(new WordPressOptions
        {
            BaseUrl = "https://example.com",
            Username = "usuario",
            ApplicationPassword = "xxxx xxxx xxxx",
            PostsEndpoint = "/wp-json/wp/v2/posts",
            DefaultPostsPageSize = 10,
            TimeoutSeconds = 5
        });

        var service = new WordPressService(client, options);
        var post = await service.GetPostBySlugAsync("post-prueba", CancellationToken.None);

        Assert.Equal(10, post.Id);
        Assert.Equal("post-prueba", post.Slug);
        Assert.Equal("Contenido completo prueba", post.Contenido);
        Assert.Equal("https://example.com/images/post-prueba.jpg", post.ImagenDestacadaUrl);
    }

    [Fact]
    public async Task GetPostBySlugAsync_ShouldThrowNotFound_WhenSlugDoesNotExist()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]", Encoding.UTF8, "application/json")
        });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var options = Options.Create(new WordPressOptions
        {
            BaseUrl = "https://example.com",
            Username = "usuario",
            ApplicationPassword = "xxxx xxxx xxxx",
            PostsEndpoint = "/wp-json/wp/v2/posts",
            DefaultPostsPageSize = 10,
            TimeoutSeconds = 5
        });

        var service = new WordPressService(client, options);
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetPostBySlugAsync("inexistente", CancellationToken.None));

        Assert.Equal("El post de WordPress no existe.", exception.Message);
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(responseFactory(request));
    }
}
