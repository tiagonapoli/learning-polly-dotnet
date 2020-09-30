using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace learning_polly_dotnet.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<WeatherForecast> Get(CancellationToken cancellationToken)
        {
            var timespan = TimeSpan.FromMilliseconds(250);
            var retryPolicy = Polly.Policy
                .Handle<Polly.Timeout.TimeoutRejectedException>()
                .RetryAsync(2, (exception, i, ctx) =>
                {
                    Console.WriteLine($"OnRetry: {i}");
                    Console.WriteLine($"message: {exception.Message}");
                    Console.WriteLine($"correlationId: {ctx.CorrelationId}");
                    Console.WriteLine($"operationKey: {ctx.OperationKey}");
                    Console.WriteLine($"policyKey: {ctx.PolicyKey}");
                    Console.WriteLine(ctx.Keys);
                    Console.WriteLine("");
                });
            
            var timeoutPolicy = Polly.Policy.TimeoutAsync(timespan, TimeoutStrategy.Optimistic);
            
            // Create retries, each with the defined timeout
            var execPolicy = Polly.Policy.WrapAsync(retryPolicy, timeoutPolicy);
            
            // Create timeout for all retries
            // var execPolicy = Polly.Policy.WrapAsync(timeoutPolicy, retryPolicy);
            
            
            try
            {
                var res = await execPolicy.ExecuteAsync((ct) => WeatherForecast.GetForecast(1, ct),
                    cancellationToken);
                return res;
            }
            catch (Polly.Timeout.TimeoutRejectedException tex)
            {
                Console.Error.WriteLine("Timeout exception!!");
                Console.Error.WriteLine(tex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.GetType());
                return null;
            }
        }
    }
}
