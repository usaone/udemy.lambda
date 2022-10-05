using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

namespace udemy.lambda.Tests;

public class FunctionTest
{
    [Fact]
    public void TestToC2FFunction()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function();
        var context = new TestLambdaContext();
        var temp = new Temperature { Value = 0.0, CorF = "C" };
        var result = function.C2FandF2C(temp, context);

        Assert.Equal(32.0, result.Value);
        Assert.Equal("F", result.CorF);
    }

    [Fact]
    public void TestToF2CFunction()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function();
        var context = new TestLambdaContext();
        var temp = new Temperature { Value = 32.0, CorF = "F" };
        var result = function.C2FandF2C(temp, context);

        Assert.Equal(0.0, result.Value);
        Assert.Equal("C", result.CorF);
    }
}

