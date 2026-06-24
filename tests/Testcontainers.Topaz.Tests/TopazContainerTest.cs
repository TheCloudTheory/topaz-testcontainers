using Testcontainers.Topaz;

namespace Testcontainers.Topaz.Tests;

/// <summary>
/// Integration tests for <see cref="TopazContainer" />.
/// Requires Docker. DNS and certificate trust are not required for these tests.
/// See README.md for full suite setup (dnsmasq + <see cref="TopazContainer.InstallCertificateToCurrentUserStore" />).
/// </summary>
public sealed class TopazContainerTest : IAsyncLifetime
{
    private readonly TopazContainer _container = new TopazBuilder().Build();

    public Task InitializeAsync() => _container.StartAsync();

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    [Fact]
    public void AllPortsAreMapped()
    {
        Assert.True(_container.GetMappedPublicPort(TopazContainer.ResourceManagerPort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.KeyVaultPort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.EventHubHttpPort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.StoragePort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.AppServicePort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.CosmosDbPort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.ContainerRegistryPort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.ServiceBusAmqpPort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.ServiceBusHttpPort) > 0);
        Assert.True(_container.GetMappedPublicPort(TopazContainer.EventHubAmqpPort) > 0);
    }

    [Fact]
    public void UriHelpersReturnCorrectHostnames()
    {
        var port = _container.GetMappedPublicPort(TopazContainer.ResourceManagerPort);
        Assert.Equal(new Uri($"https://topaz.local.dev:{port}"), _container.GetResourceManagerUri());

        port = _container.GetMappedPublicPort(TopazContainer.StoragePort);
        Assert.Equal(new Uri($"https://myaccount.blob.storage.topaz.local.dev:{port}"), _container.GetStorageBlobUri("myaccount"));

        port = _container.GetMappedPublicPort(TopazContainer.ServiceBusAmqpPort);
        Assert.Equal(new Uri($"amqp://myns.servicebus.topaz.local.dev:{port}"), _container.GetServiceBusAmqpUri("myns"));
    }

    [Fact]
    public void CertificatePemIsAvailable()
    {
        var pem = TopazContainer.GetCertificatePem();
        Assert.Contains("BEGIN CERTIFICATE", pem);
    }
}
