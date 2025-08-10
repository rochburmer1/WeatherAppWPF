namespace WeatherApp.Models
{
    public class WeatherInfo
    {
        public double Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public string Description { get; set; }
        public string IconCode { get; set; }
    }
}