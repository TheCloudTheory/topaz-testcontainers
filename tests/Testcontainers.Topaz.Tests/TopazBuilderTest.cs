using Testcontainers.Topaz;

namespace Testcontainers.Topaz.Tests;

/// <summary>
/// Unit tests for <see cref="TopazBuilder" /> option wiring.
/// These do not require Docker.
/// </summary>
public sealed class TopazBuilderTest
{
    [Fact]
    public void WithDefaultSubscription_SetsSubscriptionId()
    {
        var id = Guid.NewGuid();
        var builder = new TopazBuilder().WithDefaultSubscription(id);
        Assert.Equal(id, builder.DefaultSubscription);
    }

    [Fact]
    public void DefaultSubscription_IsNullBeforeBuild()
    {
        var builder = new TopazBuilder();
        Assert.Null(builder.DefaultSubscription);
    }

    [Fact]
    public void WithLogLevel_SetsLogLevel()
    {
        var builder = new TopazBuilder().WithLogLevel(TopazLogLevel.Debug);
        Assert.Equal(TopazLogLevel.Debug, builder.LogLevel);
    }

    [Fact]
    public void WithLoggingToFile_EnablesLogging()
    {
        var builder = new TopazBuilder().WithLoggingToFile();
        Assert.True(builder.EnableLoggingToFile);
        Assert.Null(builder.RefreshLog); // default: not explicitly set to false
    }

    [Fact]
    public void WithLoggingToFile_RefreshLogFalse_SetsRefreshLog()
    {
        var builder = new TopazBuilder().WithLoggingToFile(refreshLog: false);
        Assert.True(builder.EnableLoggingToFile);
        Assert.Equal(false, builder.RefreshLog);
    }

    [Fact]
    public void WithEmulatorIpAddress_SetsAddress()
    {
        var builder = new TopazBuilder().WithEmulatorIpAddress("0.0.0.0");
        Assert.Equal("0.0.0.0", builder.EmulatorIpAddress);
    }

    [Fact]
    public void WithDefaultSubscription_OverridesWhenCalledMultipleTimes()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var builder = new TopazBuilder()
            .WithDefaultSubscription(first)
            .WithDefaultSubscription(second);
        Assert.Equal(second, builder.DefaultSubscription);
    }
}
