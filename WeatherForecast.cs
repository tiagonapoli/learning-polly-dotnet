using System;
using System.Threading;
using System.Threading.Tasks;

namespace learning_polly_dotnet
{
    public class WeatherForecast
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        public static async Task<WeatherForecast> GetForecast(int day, CancellationToken ct)
        {
            Console.WriteLine("Executed GetForecast");
            var rng = new Random();
            await Task.Delay(500);
            ct.ThrowIfCancellationRequested();
            
            return new WeatherForecast
            {
                Date = DateTime.Now.AddDays(day),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };
        }
        
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
