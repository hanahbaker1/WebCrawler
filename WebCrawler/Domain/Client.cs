using System.Diagnostics;
using Polly;
using Polly.Wrap;

namespace WebCrawler.Domain;

class Client
{
    private readonly HttpClient _httpClient;
    private const int RetryAttempts = 2;

    internal Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
 
    internal async Task<string?> DownloadPage(Uri url)
    {
        using CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        cancelTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        CancellationToken token = cancelTokenSource.Token;
        var retryPolicy = CreateExponentialBackoffPolicy();
        try
        {
            var htmlResponse = await retryPolicy
                .ExecuteAsync(() => _httpClient.GetAsync(url, token));
            var contentType = htmlResponse.Content.Headers.ContentType;
            var response = contentType?.MediaType?.Contains("text/html") == true
                ? await htmlResponse.Content.ReadAsStringAsync(token)
                : null;
            return response;
        }
        catch (Exception e)
        {
            Trace.WriteLine($"Error while downloading page {url}, error message {e}");
            return null;
        }
    }
       
    internal AsyncPolicyWrap CreateExponentialBackoffPolicy()
    {
        var timeoutRetryPolicy = Policy
            .Handle<OperationCanceledException>()
            .WaitAndRetryAsync(
                RetryAttempts,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        var rateLimitPolicy = Policy.RateLimitAsync(3, TimeSpan.FromMilliseconds(90000));
        return timeoutRetryPolicy.WrapAsync(rateLimitPolicy);
    }
}