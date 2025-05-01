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

## JPM-2: Setup SNS Subscriber using protocol `Email-JSON`
- Create a new `SNS Subscription` for above topic in AWS
    - Choose a protocol e.g `Email-JSON`. Enter an email address and confirm the subscription
    - Make a call to the Weather Forecast Http Post action endpoint, this will trigger an email to be sent to the nominated email address


## Reference
Tutorial origin from Rahul Nath Youtube Video course on AWS SNS - https://www.youtube.com/watch?v=XVQwgeUWXVY&list=PL01_mtrYJhC15qxPDg-BjKFNeuruqPGMI&index=1&t=211s&ab_channel=RahulNath