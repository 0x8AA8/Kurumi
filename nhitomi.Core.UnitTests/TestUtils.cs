using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace nhitomi.Core.UnitTests
{
    public static class TestUtils
    {
        public static ILogger<T> Logger<T>() => new NullLogger<T>();

        public static JsonSerializer Serializer => JsonSerializer.Create(new nhitomiSerializerSettings());

        public static IHttpClient HttpClient => new TestHttpClient();
    }

    /// <summary>
    /// Simple HTTP client for unit tests.
    /// </summary>
    public class TestHttpClient : IHttpClient
    {
        public HttpClient Http { get; } = new HttpClient();

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
            => Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }
}