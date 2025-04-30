# PubSubWithSNS Example
An example of how to code using a publisher subscriber architecture in Dotnet 8, C# with AWS SNS

## JPM-1: Setup SNS Publisher logic
- Create a new `SNS Topic` in AWS
- Create a new `IAM user` with the relevant permissions to the SNS topic
- Create the publisher in DotNet:
```
[HttpPost]
public async Task Post([FromBody] WeatherForecast weatherForecast)
{
    var credentials = new BasicAWSCredentials("{IAM Access Key}", "{IAM Secret Key}");
    var client = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.APSoutheast2);
    var request = new PublishRequest
    {
        TopicArn = "{Your Topic ARN from your AWS Topic}",
        Message = JsonSerializer.Serialize(weatherForecast),
        Subject = "Weather Update"
    };
    var response = await client.PublishAsync(request);
}
```

## Reference
Tutorial origin from Rahul Nath Youtube Video course on AWS SNS - https://www.youtube.com/watch?v=XVQwgeUWXVY&list=PL01_mtrYJhC15qxPDg-BjKFNeuruqPGMI&index=1&t=211s&ab_channel=RahulNath