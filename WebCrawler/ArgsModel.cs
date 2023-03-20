using CommandLine;

namespace WebCrawler;

internal class ArgsModel
{
    [Option("url", Required = true, HelpText = "The URL of the site you're exploring. Example: https://www.example.com")]
    public string Url { get; set; } = null!;

    [Option("maxdepth", Required = false,
        HelpText = "An upper limit on the number of sub-sites within a single domain to search.")]
    public int MaxDepth { get; set; } = 4;
}