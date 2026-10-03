using Domain.Common;

namespace Tests.Common;

public class ResultTests
{
    private static readonly Error SomeError = new("Test.Error", "Something went wrong.", ErrorType.Conflict);

    [Fact]
    public void Success_HasNoError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Error_ConvertsToFailedResult()
    {
        Result result = SomeError;

        Assert.True(result.IsFailure);
        Assert.Same(SomeError, result.Error);
    }

    [Fact]
    public void ValueResult_ExposesValueOnSuccess()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ValueResult_SuccessFactory_WorksForInterfaceTypes()
    {
        IReadOnlyList<int> items = [1, 2];

        var result = Result<IReadOnlyList<int>>.Success(items);

        Assert.True(result.IsSuccess);
        Assert.Same(items, result.Value);
    }

    [Fact]
    public void ValueResult_ThrowsOnValueAccessAfterFailure()
    {
        Result<int> result = SomeError;

        Assert.True(result.IsFailure);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }
}
