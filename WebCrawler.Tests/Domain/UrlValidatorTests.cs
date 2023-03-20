using FluentAssertions;
using WebCrawler.Domain;

namespace WebCrawler.Tests.Domain;

[TestFixture]
public class UrlValidatorTests
{
    private readonly UrlValidatorReplica _sut = new();
    private readonly Uri _hostUrl = new("http://example.com");

    [TestCase("https://example.com")]
    [TestCase("http://example.com/")]
    [TestCase("https://example.com/#mainContent")]
    [TestCase("https://example.com/something.pdf")]
    [TestCase("https://example.com/something.jpeg")]
    [TestCase("https://example.com/something.mp4")]
    [TestCase("https://example.com/something.mp3")]
    [TestCase("https://example.com/something.mov")]
    [TestCase("https://example.com/something.avi")]
    public void ShouldVisit_WhenHostIsBaseUrl_ReturnsTrue(string link)
    {
        // Arrange
        var uri = new Uri(link);
        
        // Act
        var shouldVisit = _sut.ShouldVisitWrapper(uri, _hostUrl);
        
        // Assert
        shouldVisit.Should().BeTrue();
    }

    [TestCase("https://twitter.com/")]
    [TestCase("https://www.facebook.com/example")]
    [TestCase("https://community.example.com/")]
    public void ShouldVisit_WhenHostIsNotBaseUrl_ReturnsFalse(string link)
    {
        // Arrange
        var uri = new Uri(link);
        
        // Act
        var shouldVisit = _sut.ShouldVisitWrapper(uri, _hostUrl);

        // Assert
        shouldVisit.Should().BeFalse();
    }
    
    [TestCase("https://example.com")]
    [TestCase("http://example.com/")]
    [TestCase("https://example.com/#mainContent")]
    [TestCase("https://example.com/something.pdf")]
    public void TryParseLink_WhenLinkIsValid_ReturnsTrue(string link)
    {
        // Arrange
        Uri.TryCreate(link, UriKind.Absolute, out var testStub);
        
        // Act
        var tryParseLink = _sut.ParseLinkWrapper(link, out var testOutput);
        
        // Assert
        tryParseLink.Should().BeTrue();
        testOutput.Should().Be(testStub);
    }
    
    [TestCase("tel:https://example.com")]
    [TestCase("mailto:name@email.com")]
    [TestCase("https://#mainContent")]
    [TestCase("sms:https://example.com")]
    [TestCase("#mainContent")]
    public void TryParseLink_WhenLinkIsInvalid_ReturnsFalse(string link)
    {
        // Act
        var tryParseLink = _sut.ParseLinkWrapper(link, out var testOutput);
        
        // Assert
        tryParseLink.Should().BeFalse();
        testOutput.Should().BeNull();
    }
}

class UrlValidatorReplica : UrlValidator
{
    protected internal bool ShouldVisitWrapper(Uri link, Uri originalUrl)
    {
        return ShouldVisitUrl(link, originalUrl);
    }

    protected internal bool ParseLinkWrapper(string link, out Uri result)
    {
        return ParseLink(link, out result);
    }
}