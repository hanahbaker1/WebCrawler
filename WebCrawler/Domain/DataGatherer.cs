using AngleSharp;
using AngleSharp.Html.Dom;

namespace WebCrawler.Domain;

class DataGatherer
{
    internal async Task<IReadOnlyList<Uri>?> GetLinks(Uri uri, string? html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return null;
        }

        var context = BrowsingContext.New(Configuration.Default);
        //Address is used to convert relative paths to absolute paths, e.g. /about -> https://example.com/about
        var document = await context.OpenAsync(req => req.Address(uri).Content(html));

        var result = new List<Uri>();
        foreach (var link in document.GetElementsByTagName("a").OfType<IHtmlAnchorElement>().Select(x => x.Href)
                     .Distinct())
        {
            if (UrlValidator.TryParseLink(link, out var parsed))
            {
                result.Add(parsed);
            }
        }

        return result;
    }
}