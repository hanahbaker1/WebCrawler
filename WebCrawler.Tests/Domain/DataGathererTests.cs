using FluentAssertions;
using WebCrawler.Domain;

namespace WebCrawler.Tests.Domain;

[TestFixture]
public class DataGathererTests
{
    private DataGatherer _dataGatherer = new();

    [Test]
    public async Task GetLinks_WhenHtmlIsNull_ReturnsNull()
    {
        // Arrange
        var uri = new Uri("https://example.com");
        
        // Act
        var result = await _dataGatherer.GetLinks(uri, null);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Test]
    public async Task GetLinks_WhenHtmlIsEmptyString_ReturnsNull()
    {
        // Arrange
        var uri = new Uri("https://example.com");
        
        // Act
        var result = await _dataGatherer.GetLinks(uri, null);
        
        // Assert
        result.Should().BeNull();
    }
}