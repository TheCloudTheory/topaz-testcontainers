# Testcontainers.Topaz

A [Testcontainers](https://dotnet.testcontainers.org/) module for [Topaz](https://topaz.thecloudtheory.com) — the local Azure emulator.

## Installation

```shell
dotnet add package TheCloudTheory.Topaz.Testcontainers
```

## Prerequisites

### 1. DNS resolution

Topaz uses `topaz.local.dev` as its base hostname. Service endpoints use subdomains
(e.g. `myaccount.blob.storage.topaz.local.dev`). Configure a wildcard DNS resolver so
all subdomains resolve automatically:

**macOS / Linux (dnsmasq)**

```shell
# Install
brew install dnsmasq        # macOS
sudo apt install dnsmasq    # Debian/Ubuntu

# Route all *.topaz.local.dev to localhost
echo "address=/.topaz.local.dev/127.0.0.1" | sudo tee /etc/dnsmasq.d/topaz.conf

# Restart
sudo brew services restart dnsmasq   # macOS
sudo systemctl restart dnsmasq       # Linux
```

On macOS, also add a resolver file so the system uses dnsmasq for this domain:

```shell
sudo mkdir -p /etc/resolver
echo "nameserver 127.0.0.1" | sudo tee /etc/resolver/topaz.local.dev
```

**Fallback: `/etc/hosts`**

If you cannot install a DNS resolver, add an entry for each hostname your tests use:

```
127.0.0.1 topaz.local.dev
127.0.0.1 myaccount.blob.storage.topaz.local.dev
127.0.0.1 myvault.vault.topaz.local.dev
```

This requires a new entry per resource name and does not support wildcards.

### 2. Certificate trust

Topaz uses a self-signed TLS certificate. Install it into the current user's trusted
root store by calling `InstallCertificateToCurrentUserStoreAsync()` after the container
starts, and remove it on teardown with `UninstallCertificateFromCurrentUserStoreAsync()`.
See the usage example below — no certificate validation bypass is needed.

## Usage

```csharp
public sealed class MyServiceTests : IAsyncLifetime
{
    private readonly TopazContainer _topaz = new TopazBuilder().Build();

    public async Task InitializeAsync()
    {
        await _topaz.StartAsync();

        // Installs the Topaz cert into CurrentUser\Root so HttpClient and
        // Azure SDK clients trust it without disabling certificate validation.
        TopazContainer.InstallCertificateToCurrentUserStore();
    }

    public async Task DisposeAsync()
    {
        TopazContainer.UninstallCertificateFromCurrentUserStore();
        await _topaz.DisposeAsync();
    }

    [Fact]
    public async Task StorageTest()
    {
        var blobUri = _topaz.GetStorageBlobUri("myaccount");
        // Use blobUri with Azure.Storage.Blobs.BlobServiceClient ...

        var cosmosUri = _topaz.GetCosmosDbUri("mycosmosaccount");
        // Use cosmosUri with Microsoft.Azure.Cosmos.CosmosClient ...
    }
}
```

## Container-to-container setup

When your tests run inside a Docker container (e.g. CI), attach both containers to a
shared network and inject the Topaz certificate into the secondary container's CA bundle.
This mirrors the pattern used in the [Topaz.Tests.AzureCLI](https://github.com/TheCloudTheory/topaz/tree/main/Topaz.Tests.AzureCLI) fixture:

```csharp
var network = new NetworkBuilder().WithName(Guid.NewGuid().ToString("D")).Build();

var topaz = new TopazBuilder()
    .WithNetwork(network)
    .Build();

await topaz.StartAsync();

var certPem = await topaz.GetCertificatePemAsync();

var myContainer = new ContainerBuilder()
    .WithNetwork(network)
    .WithExtraHost("topaz.local.dev", topaz.IpAddress)
    .WithExtraHost("myaccount.blob.storage.topaz.local.dev", topaz.IpAddress)
    // Add one WithExtraHost per subdomain your tests use
    .WithResourceMapping(Encoding.UTF8.GetBytes(certPem), "/tmp/topaz.crt")
    // Trust the cert inside the container (example for Python/requests):
    .WithEnvironment("REQUESTS_CA_BUNDLE", "/usr/lib64/az/lib/python3.12/site-packages/certifi/cacert.pem")
    .Build();

await myContainer.StartAsync();

// Append the cert to the CA bundle inside the container:
await myContainer.ExecAsync(["/bin/sh", "-c",
    "cat /tmp/topaz.crt >> /usr/lib64/az/lib/python3.12/site-packages/certifi/cacert.pem"]);

// For Go-based tools (Terraform providers, etc.) use SSL_CERT_FILE:
// SSL_CERT_FILE=/tmp/combined.pem <command>
```

## Exposed ports

| Constant | Port | Service |
|---|---|---|
| `ResourceManagerPort` | 8899 | Azure Resource Manager |
| `KeyVaultPort` | 8898 | Key Vault |
| `StoragePort` | 8891 | Blob / Queue / Table / File Storage |
| `CosmosDbPort` | 8895 | Cosmos DB |
| `ContainerRegistryPort` | 8892 | Container Registry |
| `ServiceBusAmqpPort` | 8889 | Service Bus (AMQP) |
| `ServiceBusHttpPort` | 8887 | Service Bus (HTTP) |
| `EventHubAmqpPort` | 8888 | Event Hubs (AMQP) |
| `EventHubHttpPort` | 8897 | Event Hubs (HTTP) |
| `AppServicePort` | 8896 | App Service / Kudu |

All ports are mapped to random host ports. Use `GetMappedPublicPort(port)` or the
typed URI helpers (`GetStorageBlobUri`, `GetKeyVaultUri`, etc.) to retrieve them at runtime.

## URI helpers

| Method | Returns |
|---|---|
| `GetResourceManagerUri()` | `https://topaz.local.dev:{port}` |
| `GetKeyVaultUri(vaultName)` | `https://{vaultName}.vault.topaz.local.dev:{port}` |
| `GetStorageBlobUri(account)` | `https://{account}.blob.storage.topaz.local.dev:{port}` |
| `GetStorageQueueUri(account)` | `https://{account}.queue.storage.topaz.local.dev:{port}` |
| `GetStorageTableUri(account)` | `https://{account}.table.storage.topaz.local.dev:{port}` |
| `GetCosmosDbUri(account)` | `https://{account}.documents.topaz.local.dev:{port}/` |
| `GetContainerRegistryUri(name)` | `https://{name}.cr.topaz.local.dev:{port}` |
| `GetServiceBusAmqpUri(ns)` | `amqp://{ns}.servicebus.topaz.local.dev:{port}` |
| `GetServiceBusHttpUri(ns)` | `https://{ns}.servicebus.topaz.local.dev:{port}` |
| `GetEventHubAmqpUri(ns)` | `amqp://{ns}.eventhub.topaz.local.dev:{port}` |
| `GetEventHubHttpUri(ns)` | `https://{ns}.eventhub.topaz.local.dev:{port}` |
| `GetAppServiceUri(appName)` | `https://{appName}.scm.azurewebsites.topaz.local.dev:{port}` |

