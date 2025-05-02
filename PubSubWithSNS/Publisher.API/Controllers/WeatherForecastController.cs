using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Publisher.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IConfiguration _configuration;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost]
    public async Task Post([FromBody] WeatherForecast weatherForecast)
    {
        // Simulate some processing
        var credentials = new BasicAWSCredentials(_configuration["AWS_IAM_ACCESSKEY"], _configuration["AWS_IAM_SECRETKEY"]);
        var client = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.APSoutheast2);
        var request = new PublishRequest
        {
            TopicArn = _configuration["AWS_SNS_TOPIC_ARN"],
            Message = JsonSerializer.Serialize(weatherForecast),
            Subject = "Weather Update"
        };

        request.MessageAttributes = new Dictionary<string, MessageAttributeValue>
        {
            { "Month", new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = weatherForecast.Date.ToString("MMMM")
                }
            }
        };

        var response = await client.PublishAsync(request);
    }
}
