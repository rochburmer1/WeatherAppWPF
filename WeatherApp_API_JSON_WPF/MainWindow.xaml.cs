using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WeatherApp
{
    public partial class MainWindow : Window
    {
        private const string API_KEY = "9a9ce366878938edee44cdab1b0a6dc1";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetWeather_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text;
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units=metric&lang=pl";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string response = await client.GetStringAsync(url);
                    JObject weatherData = JObject.Parse(response);

                    double temp = (double)weatherData["main"]["temp"];
                    int pressure = (int)weatherData["main"]["pressure"];
                    int humidity = (int)weatherData["main"]["humidity"];
                    string description = weatherData["weather"][0]["description"].ToString();
                    string iconCode = weatherData["weather"][0]["icon"].ToString();

                    TemperatureText.Text = $"Temperatura: {temp}°C";
                    PressureText.Text = $"Ciśnienie: {pressure} hPa";
                    HumidityText.Text = $"Wilgotność: {humidity}%";
                    DescriptionText.Text = description;

                    WeatherIcon.Source = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{iconCode}@2x.png"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: " + ex.Message);
                }
            }
        }
    }
}