using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<WeatherInfo> GetWeatherAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City name cannot be empty");

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=pl";

            string response = await _httpClient.GetStringAsync(url);
            JObject weatherData = JObject.Parse(response);

            return new WeatherInfo
            {
                Temperature = (double)weatherData["main"]["temp"],
                Pressure = (int)weatherData["main"]["pressure"],
                Humidity = (int)weatherData["main"]["humidity"],
                Description = (string)weatherData["weather"][0]["description"],
                IconCode = (string)weatherData["weather"][0]["icon"]
            };
        }
    }
}