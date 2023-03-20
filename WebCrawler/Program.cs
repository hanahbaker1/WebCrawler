// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using CommandLine;
using WebCrawler.Domain;

namespace WebCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<ArgsModel>(args).WithParsedAsync(Runner);
        }

        private static async Task Runner(ArgsModel arg)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await new RunAndCrawl().Run(new Uri(arg.Url, UriKind.Absolute), arg.MaxDepth);
            stopwatch.Stop();
            Console.WriteLine($"Visited {result.Count} in ${stopwatch.Elapsed}");
        }
    }
}