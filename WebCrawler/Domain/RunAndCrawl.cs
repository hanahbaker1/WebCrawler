using System.Collections.Concurrent;

namespace WebCrawler.Domain;

class RunAndCrawl
{
    private readonly DataGatherer _dataGatherer = new();
    private readonly Client _client = new(new HttpClient());

    internal async Task<ConcurrentDictionary<string, IReadOnlyList<Uri>?>> Run(Uri startingUrl, int maxDepth)
    {
        var visitedPages = new ConcurrentDictionary<string, bool>();
        var pageCache = new ConcurrentDictionary<string, IReadOnlyList<Uri>?>();

        visitedPages.TryAdd(startingUrl.ToString(), true);

        await ProcessPage(startingUrl, 0);

        return pageCache;

        async Task ProcessPage(Uri pageUrl, int currentDepth)
        {
            IReadOnlyList<Uri>? links;
            var cacheKey = pageUrl.ToString();
            if (pageCache.TryGetValue(cacheKey, out var cachedUrls))
            {
                links = cachedUrls;
            }
            else
            {
                var pageContents = await _client.DownloadPage(pageUrl);
                var linksFromPage = await _dataGatherer.GetLinks(pageUrl, pageContents);
                pageCache.TryAdd(cacheKey, linksFromPage);

                links = linksFromPage;
            }

            Console.WriteLine($"Visited {pageUrl}");

            if (links is not null)
            {
                foreach (var link in links)
                {
                    Console.WriteLine($"       -> {link}");
                }

                if (currentDepth < maxDepth)
                {
                    await Task.WhenAll(
                        links.Where(l => UrlValidator.ShouldVisit(l, startingUrl))
                            .Select(Normalize)
                            .Where(l => visitedPages.TryAdd(l.ToString(), true))
                            .Select(async l => await ProcessPage(l, currentDepth + 1)));
                }
            }
        }
    }

    private static Uri Normalize(Uri link)
    { 
        //http://www.example.org/foo.html#bar
        var builder = new UriBuilder(link)
        {
            Fragment = null
        };
        return builder.Uri;
    }
}