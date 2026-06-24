using System.Text;
using Docker.DotNet.Models;

namespace Testcontainers.Topaz;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
public sealed class TopazBuilder : ContainerBuilder<TopazBuilder, TopazContainer, TopazConfiguration>
{
    public const string TopazImage = "thecloudtheory/topaz-host";

    /// <summary>
    /// Initializes a new instance of the <see cref="TopazBuilder" /> class.
    /// </summary>
    public TopazBuilder()
        : this(new TopazConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    private TopazBuilder(TopazConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override TopazConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override TopazContainer Build()
    {
        Validate();
        return new TopazContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override TopazBuilder Init()
    {
        var (certPem, keyPem) = ReadEmbeddedCerts();

        return base.Init()
            .WithImage(TopazImage)
            .WithPortBinding(TopazContainer.ResourceManagerPort, true)
            .WithPortBinding(TopazContainer.KeyVaultPort, true)
            .WithPortBinding(TopazContainer.EventHubHttpPort, true)
            .WithPortBinding(TopazContainer.StoragePort, true)
            .WithPortBinding(TopazContainer.AppServicePort, true)
            .WithPortBinding(TopazContainer.CosmosDbPort, true)
            .WithPortBinding(TopazContainer.ContainerRegistryPort, true)
            .WithPortBinding(TopazContainer.ServiceBusAmqpPort, true)
            .WithPortBinding(TopazContainer.ServiceBusHttpPort, true)
            .WithPortBinding(TopazContainer.EventHubAmqpPort, true)
            .WithResourceMapping(Encoding.UTF8.GetBytes(certPem), "/app/topaz.crt")
            .WithResourceMapping(Encoding.UTF8.GetBytes(keyPem), "/app/topaz.key")
            .WithCommand(
                "--certificate-file", "topaz.crt",
                "--certificate-key", "topaz.key",
                "--default-subscription", Guid.NewGuid().ToString())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Topaz is ready"));
    }

    /// <inheritdoc />
    protected override TopazBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        => Merge(DockerResourceConfiguration, new TopazConfiguration(resourceConfiguration));

    /// <inheritdoc />
    protected override TopazBuilder Clone(IContainerConfiguration resourceConfiguration)
        => Merge(DockerResourceConfiguration, new TopazConfiguration(resourceConfiguration));

    /// <inheritdoc />
    protected override TopazBuilder Merge(TopazConfiguration oldValue, TopazConfiguration newValue)
        => new(new TopazConfiguration(oldValue, newValue));

    private static (string CertPem, string KeyPem) ReadEmbeddedCerts()
    {
        var assembly = typeof(TopazBuilder).Assembly;
        using var certStream = assembly.GetManifestResourceStream("Testcontainers.Topaz.topaz.crt")!;
        using var keyStream = assembly.GetManifestResourceStream("Testcontainers.Topaz.topaz.key")!;
        using var certReader = new StreamReader(certStream);
        using var keyReader = new StreamReader(keyStream);
        return (certReader.ReadToEnd(), keyReader.ReadToEnd());
    }

}
