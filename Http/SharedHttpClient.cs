using System;
using System.Net.Http;

namespace MairiesHub.Http;

public sealed class SharedHttpClient
{
    private static readonly Lazy<SharedHttpClient> _lazy =
        new(() => new SharedHttpClient(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

    public static SharedHttpClient Instance => _lazy.Value;

    public HttpClient Client { get; }

    private SharedHttpClient()
    {
        var baseUrl = Environment.GetEnvironmentVariable("MARIESHUB_API")
                      ?? "http://localhost:8765";
        Client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(10)
        };
    }
}
