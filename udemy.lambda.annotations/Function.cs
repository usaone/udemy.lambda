using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace udemy.lambda.annotations;

public class Function
{
    private readonly DynamoDBContext _dynamoDBContext;

    public Function()
    {
        _dynamoDBContext = new DynamoDBContext(new AmazonDynamoDBClient());
    }   

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "users/{userId}")]
    public async Task<User> FunctionHandler(string userId, ILambdaContext context)
    {
        Guid.TryParse(userId, out var id);
        LambdaLogger.Log($"Test Message for userId {userId}");
        return await _dynamoDBContext.LoadAsync<User>(id);
    }
}
