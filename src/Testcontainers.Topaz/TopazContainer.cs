namespace Testcontainers.Topaz;

/// <inheritdoc cref="DockerContainer" />
public sealed class TopazContainer : DockerContainer
{
    /// <summary>Port for the Azure Resource Manager endpoint.</summary>
    public const ushort ResourceManagerPort = 8899;

    /// <summary>Port for the Key Vault endpoint.</summary>
    public const ushort KeyVaultPort = 8898;

    /// <summary>Port for the Event Hub HTTP endpoint.</summary>
    public const ushort EventHubHttpPort = 8897;

    /// <summary>Port for the Storage (Blob, Queue, Table, File) endpoint.</summary>
    public const ushort StoragePort = 8891;

    /// <summary>Port for the App Service / Kudu endpoint.</summary>
    public const ushort AppServicePort = 8896;

    /// <summary>Port for the Cosmos DB endpoint.</summary>
    public const ushort CosmosDbPort = 8895;

    /// <summary>Port for the Container Registry endpoint.</summary>
    public const ushort ContainerRegistryPort = 8892;

    /// <summary>Port for the Service Bus AMQP endpoint.</summary>
    public const ushort ServiceBusAmqpPort = 8889;

    /// <summary>Port for the Service Bus HTTP endpoint.</summary>
    public const ushort ServiceBusHttpPort = 8887;

    /// <summary>Port for the Event Hub AMQP endpoint.</summary>
    public const ushort EventHubAmqpPort = 8888;

    /// <summary>Port for the HTTP CONNECT proxy (used for ROPC <c>az login</c> from the host).</summary>
    public const ushort ConnectProxyPort = 44380;

    /// <summary>
    /// Initializes a new instance of the <see cref="TopazContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public TopazContainer(TopazConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Azure Resource Manager base URI (e.g. <c>https://topaz.local.dev:PORT</c>).
    /// </summary>
    public Uri GetResourceManagerUri()
        => new($"https://topaz.local.dev:{GetMappedPublicPort(ResourceManagerPort)}");

    /// <summary>
    /// Gets the Key Vault DNS suffix used for vault URIs: <c>{vaultName}.vault.topaz.local.dev:PORT</c>.
    /// Build the full URI with <c>https://{vaultName}.vault.topaz.local.dev:{GetMappedPublicPort(KeyVaultPort)}</c>.
    /// </summary>
    public Uri GetKeyVaultUri(string vaultName)
        => new($"https://{vaultName}.vault.topaz.local.dev:{GetMappedPublicPort(KeyVaultPort)}");

    /// <summary>
    /// Gets the Blob Storage URI for the given account.
    /// </summary>
    public Uri GetStorageBlobUri(string accountName)
        => new($"https://{accountName}.blob.storage.topaz.local.dev:{GetMappedPublicPort(StoragePort)}");

    /// <summary>
    /// Gets the Queue Storage URI for the given account.
    /// </summary>
    public Uri GetStorageQueueUri(string accountName)
        => new($"https://{accountName}.queue.storage.topaz.local.dev:{GetMappedPublicPort(StoragePort)}");

    /// <summary>
    /// Gets the Table Storage URI for the given account.
    /// </summary>
    public Uri GetStorageTableUri(string accountName)
        => new($"https://{accountName}.table.storage.topaz.local.dev:{GetMappedPublicPort(StoragePort)}");

    /// <summary>
    /// Gets the Cosmos DB URI for the given account.
    /// </summary>
    public Uri GetCosmosDbUri(string accountName)
        => new($"https://{accountName}.documents.topaz.local.dev:{GetMappedPublicPort(CosmosDbPort)}/");

    /// <summary>
    /// Gets the Container Registry URI for the given registry name.
    /// </summary>
    public Uri GetContainerRegistryUri(string registryName)
        => new($"https://{registryName}.cr.topaz.local.dev:{GetMappedPublicPort(ContainerRegistryPort)}");

    /// <summary>
    /// Gets the Service Bus AMQP URI for the given namespace.
    /// </summary>
    public Uri GetServiceBusAmqpUri(string namespaceName)
        => new($"amqp://{namespaceName}.servicebus.topaz.local.dev:{GetMappedPublicPort(ServiceBusAmqpPort)}");

    /// <summary>
    /// Gets the Service Bus HTTP URI for the given namespace.
    /// </summary>
    public Uri GetServiceBusHttpUri(string namespaceName)
        => new($"https://{namespaceName}.servicebus.topaz.local.dev:{GetMappedPublicPort(ServiceBusHttpPort)}");

    /// <summary>
    /// Gets the Event Hub AMQP URI for the given namespace.
    /// </summary>
    public Uri GetEventHubAmqpUri(string namespaceName)
        => new($"amqp://{namespaceName}.eventhub.topaz.local.dev:{GetMappedPublicPort(EventHubAmqpPort)}");

    /// <summary>
    /// Gets the Event Hub HTTP URI for the given namespace.
    /// </summary>
    public Uri GetEventHubHttpUri(string namespaceName)
        => new($"https://{namespaceName}.eventhub.topaz.local.dev:{GetMappedPublicPort(EventHubHttpPort)}");

    /// <summary>
    /// Gets the App Service / Kudu URI for the given app name.
    /// </summary>
    public Uri GetAppServiceUri(string appName)
        => new($"https://{appName}.scm.azurewebsites.topaz.local.dev:{GetMappedPublicPort(AppServicePort)}");

    /// <summary>
    /// Returns the PEM-encoded certificate bundled with this package.
    /// Use this to inject the cert into secondary containers via
    /// <c>WithResourceMapping(Encoding.UTF8.GetBytes(pem), "/tmp/topaz.crt")</c>.
    /// </summary>
    public static string GetCertificatePem()
    {
        var assembly = typeof(TopazContainer).Assembly;
        using var stream = assembly.GetManifestResourceStream("Testcontainers.Topaz.topaz.crt")!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Returns the Topaz self-signed certificate as an <see cref="X509Certificate2" />.
    /// </summary>
    public static X509Certificate2 GetCertificate()
        => X509Certificate2.CreateFromPem(GetCertificatePem());

    /// <summary>
    /// Installs the Topaz self-signed certificate into the current user's trusted root store.
    /// Call this in your test suite's <c>OneTimeSetUp</c> / fixture constructor so that
    /// <see cref="System.Net.Http.HttpClient" /> and Azure SDK clients can verify the TLS
    /// certificate without disabling certificate validation.
    /// </summary>
    /// <remarks>
    /// The certificate is scoped to <see cref="StoreLocation.CurrentUser" /> and does not
    /// require elevated privileges. Remove it afterwards with
    /// <see cref="UninstallCertificateFromCurrentUserStoreAsync" /> in teardown.
    /// </remarks>
    /// <summary>
    /// Installs the Topaz self-signed certificate into the current user's trusted root store.
    /// Call this in your test suite's <c>OneTimeSetUp</c> / fixture constructor so that
    /// <see cref="System.Net.Http.HttpClient" /> and Azure SDK clients can verify the TLS
    /// certificate without disabling certificate validation.
    /// </summary>
    /// <remarks>
    /// The certificate is scoped to <see cref="StoreLocation.CurrentUser" /> and does not
    /// require elevated privileges. Remove it afterwards with
    /// <see cref="UninstallCertificateFromCurrentUserStore" /> in teardown.
    /// </remarks>
    public static void InstallCertificateToCurrentUserStore()
    {
        using var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadWrite);
        store.Add(GetCertificate());
    }

    /// <summary>
    /// Removes the Topaz certificate from the current user's trusted root store.
    /// Call this in your test suite's <c>OneTimeTearDown</c>.
    /// </summary>
    public static void UninstallCertificateFromCurrentUserStore()
    {
        using var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadWrite);
        store.Remove(GetCertificate());
    }
}
