using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using WeatherApp.Services;
using Xunit;

namespace WeatherApp.Tests
{
    public class WeatherServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetWeatherAsync_EmptyCity_ThrowsArgumentException(string city)
        {
            var httpClient = new HttpClient(new Mock<HttpMessageHandler>().Object);
            var service = new WeatherService(httpClient, "FAKE_API_KEY");

            await Assert.ThrowsAsync<ArgumentException>(() => service.GetWeatherAsync(city));
        }
        [Fact]
        public async Task GetWeatherAsync_HttpError_ThrowsHttpRequestException()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Not Found")
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new WeatherService(httpClient, "FAKE_API_KEY");

            // GetStringAsync rzuci HttpRequestException, ale to zależy od implementacji, więc wywołaj i sprawdź wyjątek
            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetWeatherAsync("Warsaw"));
        }
        [Fact]
        public async Task GetWeatherAsync_InvalidJson_ThrowsJsonReaderException()
        {
            var invalidJson = "Not a JSON";

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(invalidJson)
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new WeatherService(httpClient, "FAKE_API_KEY");

            await Assert.ThrowsAsync<Newtonsoft.Json.JsonReaderException>(() => service.GetWeatherAsync("Warsaw"));
        }
        [Fact]
        public async Task GetWeatherAsync_ReturnsCorrectValues_ForDifferentInput()
        {
            var json = @"{
        'main': {'temp': 10.0, 'pressure': 1000, 'humidity': 80},
        'weather': [{'description': 'rainy', 'icon': '09d'}]
    }";

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new WeatherService(httpClient, "FAKE_API_KEY");

            var result = await service.GetWeatherAsync("Gdansk");

            Assert.Equal(10.0, result.Temperature);
            Assert.Equal(1000, result.Pressure);
            Assert.Equal(80, result.Humidity);
            Assert.Equal("rainy", result.Description);
            Assert.Equal("09d", result.IconCode);
        }

        [Fact]
        public async Task GetWeatherAsync_ReturnsParsedData()
        {
            // Arrange
            var json = @"{
                'main': {'temp': 20.5, 'pressure': 1012, 'humidity': 55},
                'weather': [{'description': 'clear sky', 'icon': '01d'}]
            }";

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new WeatherService(httpClient, "FAKE_API_KEY");

            // Act
            var result = await service.GetWeatherAsync("Warsaw");

            // Assert
            Assert.Equal(20.5, result.Temperature);
            Assert.Equal(1012, result.Pressure);
            Assert.Equal(55, result.Humidity);
            Assert.Equal("clear sky", result.Description);
            Assert.Equal("01d", result.IconCode);
        }
    }
}