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

## JPM-3: Setup SNS Subscriber using protocol `AWS Lambda`
### `PREREQUISITES`: Requires the `AWS Toolkit for Visual Studio`. Install from VS Extensions - Search `AWS` and pick the AWS Toolkit appropriate for your VS version


- In VS Solution, create a new `AWS Lambda Project (.NET Core - C#)` template project

    - Choose Blueprint `Simple SNS Function`
- Publish the AWS Lambda function:
    - Right-click the Lambda project > choose `Publish to AWS Lambda...`
    - Give your lambda function a name e.g `csharppubsubmaman-pubsub-with-sns-weather-data`
    - Next, choose an execution role (for this, we chose `AWSLambdaBasicExecutionRole` from the list of AWS managed policies)
    - Upload the lambda function
- In `AWS Lambda portal`, add a trigger to detect new messages for the SNS topic
    - Select `Configuration` tab > Select `Triggers` menu > Click `Add Trigger`
    - In the select dropdown, choose `SNS` as the trigger source
    - Then, specify the topic we created earlier
- In `AWS SNS portal`, refresh the page and find a new subscription added for the lambda function
- Make a call to the `Weather Forecast Http Post action endpoint`; This will trigger our lambda function (verify that the lambda was called by going to `Monitor` tab and go to `CloudWatch logs`), additionally this will still trigger an email to be sent to the subscription with protocol `Email-JSON` with the nominated email address (recall we have two subscriptions setup, one for `AWS Lambda` and one for `Email-JSON`)

## Reference
Tutorial origin from Rahul Nath Youtube Video course on AWS SNS - https://www.youtube.com/watch?v=XVQwgeUWXVY&list=PL01_mtrYJhC15qxPDg-BjKFNeuruqPGMI&index=1&t=211s&ab_channel=RahulNath