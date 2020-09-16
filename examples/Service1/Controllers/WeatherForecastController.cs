using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory httpClientFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "http://localhost:5020/WeatherForecast?teste=1");

            var client = httpClientFactory.CreateClient("client-service2");

            await client.SendAsync(request);

            await client.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                "http://www.google.com"));


            var rng = new Random();
            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }));
        }
    }
}
