using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace udemy.lambda;

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
    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        return request.RequestContext.Http.Method.ToUpper() switch
        {
            "GET" => await HandleGetRequest(request),
            "POST" => await HandlePostRequest(request),
            "DELETE" => await HandleDeleteRequest(request)
        };
    }

    private async Task<APIGatewayHttpApiV2ProxyResponse> HandleGetRequest(APIGatewayHttpApiV2ProxyRequest request)
    {
        request.PathParameters.TryGetValue("userId", out var userIdString);
        if (Guid.TryParse(userIdString, out var userId))
        {
            var user = await _dynamoDBContext.LoadAsync<User>(userId);

            if (user != null)
            {

                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = JsonSerializer.Serialize(user),
                    StatusCode = 200
                };
            }
            else
                return NotFound404("UserId does not exist.");
        }

        return BadResponse400("Invalid UserId.");
    }

    private async Task<APIGatewayHttpApiV2ProxyResponse> HandlePostRequest(APIGatewayHttpApiV2ProxyRequest request)
    {
        var user = JsonSerializer.Deserialize<User>(request.Body);
        if (user == null)
        {
            return BadResponse400("Invalid input.");
        }

        await _dynamoDBContext.SaveAsync(user);
        return OkResponse200();
    }

    private async Task<APIGatewayHttpApiV2ProxyResponse> HandleDeleteRequest(APIGatewayHttpApiV2ProxyRequest request)
    {
        request.PathParameters.TryGetValue("userId", out var userIdString);
        if (Guid.TryParse(userIdString, out var userId))
        {
            await _dynamoDBContext.DeleteAsync<User>(userId);
            return OkResponse200();
        }

        return BadResponse400("Invalid UserId.");
    }

    private APIGatewayHttpApiV2ProxyResponse OkResponse200() => new APIGatewayHttpApiV2ProxyResponse() { StatusCode = 200 };

    private APIGatewayHttpApiV2ProxyResponse BadResponse400(string message)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            Body = "Bad Response. " + message,
            StatusCode = 400
        };
    }

    private APIGatewayHttpApiV2ProxyResponse NotFound404(string message)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            Body = "Not Found. " + message,
            StatusCode = 404
        };
    }
}
