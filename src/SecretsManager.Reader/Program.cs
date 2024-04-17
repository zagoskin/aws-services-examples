// See https://aka.ms/new-console-template for more information
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Dumpify;

var client = new AmazonSecretsManagerClient();

var request = new GetSecretValueRequest
{
    SecretId = "ApiKey"
};

var response = await client.GetSecretValueAsync(request);

if (response is null || response.HttpStatusCode is not System.Net.HttpStatusCode.OK)
{
    Console.WriteLine("Failed to retrieve secret");
    return;
}

Console.WriteLine(response.SecretString);


var describeSecretRequest = new DescribeSecretRequest
{
    SecretId = "ApiKey"
};
var describeResponse = await client.DescribeSecretAsync(describeSecretRequest);

if (describeResponse is null || describeResponse.HttpStatusCode is not System.Net.HttpStatusCode.OK)
{
    Console.WriteLine("Failed to describe secret");
    return;
}

describeResponse.Dump();