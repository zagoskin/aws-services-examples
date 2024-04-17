using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda.DynamoDB;
public class Function
{
    public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");

        foreach (var record in dynamoEvent.Records)
        {
            context.Logger.LogInformation($"Event ID: {record.EventID}");
            context.Logger.LogInformation($"Event Name: {record.EventName}");

            if (record.Dynamodb.OldImage is not null)
            {
                context.Logger.LogInformation($"Old Image: {record.Dynamodb.OldImage.ToJsonPretty()}");
            }

            
            context.Logger.LogInformation($"New Image: {record.Dynamodb.NewImage.ToJsonPretty()}");

            // TODO: Add business logic processing the record.Dynamodb object.
        }

        context.Logger.LogInformation("Stream processing complete.");
    }
}