namespace WebCrawler.Domain;

class UrlValidator
{
    internal static bool ShouldVisit(Uri link, Uri originalUrl)
    {
        return link.Host == originalUrl.Host;
    }

    internal static bool TryParseLink(string link, out Uri result)
    {
        result = null!;
        if (string.IsNullOrEmpty(link))
        {
            return false;
        }

        if (link.StartsWith("#"))
        {
            //Book marks are not wanted
            return false;
        }

        if (!Uri.TryCreate(link, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!(string.Equals(uri.Scheme, "http", StringComparison.InvariantCultureIgnoreCase) ||
              string.Equals(uri.Scheme, "https", StringComparison.InvariantCultureIgnoreCase)))
        {
            return false;
        }

        result = uri;
        return true;
    }

    //Extract and override call to use to unit test a static methods.
    protected bool ShouldVisitUrl(Uri link, Uri originalUrl)
    {
        return ShouldVisit(link, originalUrl);
    }

    protected bool ParseLink(string link, out Uri result)
    {
        return TryParseLink(link, out result);
    }
}