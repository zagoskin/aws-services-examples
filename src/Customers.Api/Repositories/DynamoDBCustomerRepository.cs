using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal;
using Customers.Api.Contracts.Data;
using Customers.Api.Extensions;
using Dumpify;
using System.Net;
using System.Text.Json;

namespace Customers.Api.Repositories;

public class DynamoDBCustomerRepository : ICustomerRepository
{
    private readonly IAmazonDynamoDB _dynamoDB;
    private readonly string _tableItem = "dome-table";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    public DynamoDBCustomerRepository(IAmazonDynamoDB dynamoDB)
    {
        _dynamoDB = dynamoDB;
    }

    public async Task<bool> CreateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerJson = Serialize(customer);
        var customerAsAttributes = Document.FromJson(customerJson).ToAttributeMap();
        var createItemRequest = new PutItemRequest(
            _tableItem,
            customerAsAttributes)
        {
            ConditionExpression = "attribute_not_exists(pk) AND attribute_not_exists(sk)"
        };

        var response = await _dynamoDB.PutItemAsync(createItemRequest);

        return response is not null && response.IsSuccessResponse();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleteItemRequest = new DeleteItemRequest(
            _tableItem,
            new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            });

        var response = await _dynamoDB.DeleteItemAsync(deleteItemRequest);
        return response is not null && response.IsSuccessResponse();
    }

    public Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var getItemRequest = new GetItemRequest(
            _tableItem,
            new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue { S = id.ToString() } },
                { "sk", new AttributeValue { S = id.ToString() } }
            });

        var response = await _dynamoDB.GetItemAsync(getItemRequest);

        if (response is null || response.Item.Count is 0 || !response.IsSuccessResponse())
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    public async Task<bool> UpdateAsync(CustomerDto customer, DateTime requestStarted)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerJson = Serialize(customer);
        var customerAsAttributes = Document.FromJson(customerJson).ToAttributeMap();
        var requestStartedString = requestStarted.ToString("O");
        var updateItemRequest = new PutItemRequest(
            _tableItem,
            customerAsAttributes)
        {
            ConditionExpression = "updatedAt < :requestStarted",
            ExpressionAttributeValues = new()
            {
                { ":requestStarted", new() { S = requestStartedString } }
            }
        };

        var response = await _dynamoDB.PutItemAsync(updateItemRequest);

        return response is not null && response.HttpStatusCode == HttpStatusCode.OK;

    }

    private string Serialize(object obj) => JsonSerializer.Serialize(obj, _jsonSerializerOptions);
    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
}
