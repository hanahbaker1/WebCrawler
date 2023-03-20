using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Polly.Wrap;
using WebCrawler.Domain;

namespace WebCrawler.Tests.Domain
{
    public class ClientTests
    {
        private Client _client = null!;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler = new();

        [SetUp]
        public void SetUp()
        {
            //To mock the behavior of the HttpClient, we use HttpMessageHandler to intercept the request and provide a mocked response.
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _client = new Client(httpClient);
            _client.CreateExponentialBackoffPolicy();
        }

        [Test]
        public async Task DownloadPage_ReturnsHtmlResponse_WhenTheServerReturnsHtml()
        {
            // Arrange
            var url = new Uri("https://example.com");
            var expectedResponse = "<html><body><h1>Hello, World!</h1></body></html>";
            var htmlContent = new StringContent(expectedResponse);
            htmlContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = htmlContent });

            // Act
            var response = await _client.DownloadPage(url);

            // Assert
            response.Should().Be(expectedResponse);
        }

        [Test]
        public async Task DownloadPage_ReturnsNull_WhenTheServerReturnsNonHtmlContent()
        {
            // Arrange
            var url = new Uri("https://example.com");
            var expectedResponse = default(string);
            var nonHtmlContent = new StringContent("text");
            nonHtmlContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = nonHtmlContent });

            // Act
            var response = await _client.DownloadPage(url);

            // Assert
            response.Should().Be(expectedResponse);
        }

        [Test]
        public void DownloadPage_ReturnsNull_WhenTheServerThrowsException()
        {
            // Arrange
            var url = new Uri("https://example.com");
            var expectedResponse = default(string);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception());

            // Act
            var response = _client.DownloadPage(url).GetAwaiter().GetResult();

            // Assert
            response.Should().Be(expectedResponse);
        }

        [Test]
        public void CreateExponentialBackoffPolicy_WhenCalled_ReturnsPolicyWrap()
        {
            // Act
            var policyWrap = _client.CreateExponentialBackoffPolicy();

            // Assert
            policyWrap.Should().NotBeNull();
            policyWrap.GetPolicies().Should().HaveCount(2);
        }
    }
}