using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;
using WeatherApp.Services;

namespace WeatherApp
{
    public partial class MainWindow : Window
    {
        private readonly WeatherService _weatherService;

        public MainWindow()
        {
            InitializeComponent();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            _weatherService = new WeatherService(new HttpClient(), "9a9ce366878938edee44cdab1b0a6dc1");
        }

        private async void GetWeather_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var city = CityTextBox.Text.Trim();
                var weather = await _weatherService.GetWeatherAsync(city);

                TemperatureText.Text = $"Temperatura: {weather.Temperature}°C";
                PressureText.Text = $"Ciśnienie: {weather.Pressure} hPa";
                HumidityText.Text = $"Wilgotność: {weather.Humidity}%";
                DescriptionText.Text = weather.Description;

                WeatherIcon.Source = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{weather.IconCode}@2x.png"));
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Problem z połączeniem internetowym lub API.", "Błąd połączenia", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                MessageBox.Show("Niepoprawna odpowiedź serwera.", "Błąd danych", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}