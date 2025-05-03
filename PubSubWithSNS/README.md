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

## JPM-4: Setup AWS SNS Message Filtering
- In our dotnet solution lambda project, update the logic in `Function.cs > private async Task ProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)` and upload a new lambda function to differentiate filter processing
    - Set the NEW lambda function name to `csharppubsubmaman-pubsub-with-sns-weather-reports` and upload
- In `AWS SNS portal`, create a new AWS Topic `Subscription` for the NEW lambda function
    - Specify the protocol `AWS Lambda`
    - Specify the endpoint to point to the `lambda function ARN` for the new lambda function we created
    - In `Subscription filter policy - optional` section, specify the JSON object to filter messages that the subscription receives:
    ```
    {
    "Month": [
        "May"
    ]
    }
    ```
- In our publisher logic, Add a message attribute to the simple notification service `publish request` to filter by the `Month` property:
```
request.MessageAttributes = new Dictionary<string, MessageAttributeValue>
{
    { "Month", new MessageAttributeValue
        {
            DataType = "String",
            StringValue = weatherForecast.Date.ToString("MMMM")
        }
    }
};
```
- Test when our lambda functions trigger after we made our changes above:
    - Make a call to the `Weather Forecast Http Post action endpoint` with the `Month` set to `May`; Both lambda functions will trigger
    - Make a call to the `Weather Forecast Http Post action endpoint` with the `Month` set to `April`; ONLY the lambda function without the filter will trigger

## JPM-5: Setup Dead Letter Queue (DLQ)
- Whenever messages are missed after x number of retries, we can opt to send our messages to a `Dead Letter Queue`. It is an SQS queue that an Amazon SNS subscription can target for messages that cannot be delivered to subscribers successfully.
- To do that, go to the `AWS SQS portal`
    - Create a new queue
    - Name it something relevant to a DLQ, something like `youtube-sns-dlq`
- In `AWS SNS portal`, assign the SQS DLQ to our latest SNS subscription (SQS DLQ is assigned at the subscription level)
    - Go to the subscription
    - Select the `Redrive policy (dead-letter queue)` tab > Click `Edit`
    - In the `Redrive policy (dead-letter queue) - optional` panel, enable the redrive policy
    - Select the DLQ we have just created above and click `Save changes` - `IMPORTANT` - an error will popup stating that we need to grant permission for SNS topic to send messages to SQS queue and to create an `Amazon SQS queue policy`
- Go back to the `AWS SQS portal` and edit the DLQ you created earlier
    - In `Access Policy` panel, modify the policy as follows, introduce a new access policy statement below:
    ```
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "sns.amazonaws.com"
      },
      "Action": "sqs:SendMessage",
      "Resource": "arn:aws:sqs:ap-southeast-2:150778450443:youtube-sns-dlq",
      "Condition": {
        "ArnEquals": {
          "aws:SourceArn": "arn:aws:sns:ap-southeast-2:150778450443:youtube-sns"
        }
      }
    }
    ```
- To test the DLQ, delete the lambda function and make a call to the `Weather Forecast Http Post action endpoint`; the SNS topic will send a message to the DLQ because it fails to find the lambda function
- `NOTE: if you need to test the lambda function again, please re-upload from VS`


## Reference
Tutorial origin from `Rahul Nath` Youtube Video course on AWS SNS - https://www.youtube.com/watch?v=XVQwgeUWXVY&list=PL01_mtrYJhC15qxPDg-BjKFNeuruqPGMI&index=1&t=211s&ab_channel=RahulNath