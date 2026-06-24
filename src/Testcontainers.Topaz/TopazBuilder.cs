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

    /// <summary>Gets the configured log level, or <c>null</c> if not set.</summary>
    public TopazLogLevel? LogLevel => DockerResourceConfiguration.LogLevel;

    /// <summary>Gets whether file logging is enabled.</summary>
    public bool EnableLoggingToFile => DockerResourceConfiguration.EnableLoggingToFile;

    /// <summary>Gets the refresh-log setting, or <c>null</c> if not set.</summary>
    public bool? RefreshLog => DockerResourceConfiguration.RefreshLog;

    /// <summary>Gets the configured default subscription ID, or <c>null</c> if not set.</summary>
    public Guid? DefaultSubscription => DockerResourceConfiguration.DefaultSubscription;

    /// <summary>Gets the configured emulator IP address, or <c>null</c> if not set.</summary>
    public string? EmulatorIpAddress => DockerResourceConfiguration.EmulatorIpAddress;

    /// <inheritdoc />
    public override TopazContainer Build()
    {
        Validate();
        return new TopazContainer(WithCommand(BuildTopazCommand()).DockerResourceConfiguration);
    }

    private string[] BuildTopazCommand()
    {
        var args = new List<string>
        {
            "--certificate-file", "topaz.crt",
            "--certificate-key", "topaz.key",
            "--default-subscription", (DockerResourceConfiguration.DefaultSubscription ?? Guid.NewGuid()).ToString()
        };

        if (DockerResourceConfiguration.LogLevel.HasValue)
        {
            args.Add("--log-level");
            args.Add(DockerResourceConfiguration.LogLevel.Value.ToString());
        }

        if (DockerResourceConfiguration.EnableLoggingToFile)
        {
            args.Add("--enable-logging-to-file");
            if (DockerResourceConfiguration.RefreshLog.HasValue && !DockerResourceConfiguration.RefreshLog.Value)
                args.Add("--refresh-log=false");
        }

        if (!string.IsNullOrEmpty(DockerResourceConfiguration.EmulatorIpAddress))
        {
            args.Add("--emulator-ip-address");
            args.Add(DockerResourceConfiguration.EmulatorIpAddress);
        }

        return args.ToArray();
    }

    /// <summary>Sets the log level for the Topaz host process.</summary>
    public TopazBuilder WithLogLevel(TopazLogLevel logLevel)
        => Merge(DockerResourceConfiguration, new TopazConfiguration(logLevel: logLevel));

    /// <summary>Enables logging to file. Optionally clears the log on startup (default: true).</summary>
    public TopazBuilder WithLoggingToFile(bool refreshLog = true)
        => Merge(DockerResourceConfiguration, new TopazConfiguration(enableLoggingToFile: true, refreshLog: refreshLog ? null : false));

    /// <summary>Sets the default subscription ID created on startup.</summary>
    public TopazBuilder WithDefaultSubscription(Guid subscriptionId)
        => Merge(DockerResourceConfiguration, new TopazConfiguration(defaultSubscription: subscriptionId));

    /// <summary>Sets the IP address the emulator listens on.</summary>
    public TopazBuilder WithEmulatorIpAddress(string ipAddress)
        => Merge(DockerResourceConfiguration, new TopazConfiguration(emulatorIpAddress: ipAddress));

    /// <inheritdoc />
    protected override TopazBuilder Init()
    {
        var (certPem, keyPem) = ReadEmbeddedCerts();

        return base.Init()
            .WithImage(TopazImage)
            .WithPortBinding(TopazContainer.ResourceManagerPort, TopazContainer.ResourceManagerPort)
            .WithPortBinding(TopazContainer.KeyVaultPort, TopazContainer.KeyVaultPort)
            .WithPortBinding(TopazContainer.EventHubHttpPort, TopazContainer.EventHubHttpPort)
            .WithPortBinding(TopazContainer.StoragePort, TopazContainer.StoragePort)
            .WithPortBinding(TopazContainer.AppServicePort, TopazContainer.AppServicePort)
            .WithPortBinding(TopazContainer.CosmosDbPort, TopazContainer.CosmosDbPort)
            .WithPortBinding(TopazContainer.ContainerRegistryPort, TopazContainer.ContainerRegistryPort)
            .WithPortBinding(TopazContainer.ServiceBusAmqpPort, TopazContainer.ServiceBusAmqpPort)
            .WithPortBinding(TopazContainer.ServiceBusHttpPort, TopazContainer.ServiceBusHttpPort)
            .WithPortBinding(TopazContainer.EventHubAmqpPort, TopazContainer.EventHubAmqpPort)
            .WithPortBinding(TopazContainer.ConnectProxyPort, TopazContainer.ConnectProxyPort)
            .WithResourceMapping(Encoding.UTF8.GetBytes(certPem), "/app/topaz.crt")
            .WithResourceMapping(Encoding.UTF8.GetBytes(keyPem), "/app/topaz.key")
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
