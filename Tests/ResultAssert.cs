using Domain.Common;

namespace Tests;

internal static class ResultAssert
{
    public static void Success(Result result) =>
        Assert.True(result.IsSuccess, $"Expected success, got {result.Error?.Code}: {result.Error?.Message}");

    public static void Failure(Result result, string expectedCode)
    {
        Assert.True(result.IsFailure, $"Expected failure {expectedCode}, got success.");
        Assert.Equal(expectedCode, result.Error!.Code);
    }
}
