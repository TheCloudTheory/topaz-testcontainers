namespace Testcontainers.Topaz;

/// <summary>Log level for the Topaz host process.</summary>
public enum TopazLogLevel
{
    Debug,
    Information,
    Warning,
    Error
}

/// <inheritdoc cref="ContainerConfiguration" />
public sealed class TopazConfiguration : ContainerConfiguration
{
    /// <summary>Gets the log level passed to the host process.</summary>
    public TopazLogLevel? LogLevel { get; }

    /// <summary>Gets whether file logging is enabled.</summary>
    public bool EnableLoggingToFile { get; }

    /// <summary>Gets whether the log file is cleared on startup.</summary>
    public bool? RefreshLog { get; }

    /// <summary>Gets the default subscription ID created on startup.</summary>
    public Guid? DefaultSubscription { get; }

    /// <summary>Gets the IP address the emulator listens on.</summary>
    public string? EmulatorIpAddress { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopazConfiguration" /> class.
    /// </summary>
    public TopazConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopazConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TopazConfiguration(IResourceConfiguration<Docker.DotNet.Models.CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopazConfiguration" /> class with Topaz-specific options.
    /// </summary>
    public TopazConfiguration(
        TopazLogLevel? logLevel = null,
        bool enableLoggingToFile = false,
        bool? refreshLog = null,
        Guid? defaultSubscription = null,
        string? emulatorIpAddress = null)
    {
        LogLevel = logLevel;
        EnableLoggingToFile = enableLoggingToFile;
        RefreshLog = refreshLog;
        DefaultSubscription = defaultSubscription;
        EmulatorIpAddress = emulatorIpAddress;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopazConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TopazConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopazConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TopazConfiguration(TopazConfiguration resourceConfiguration)
        : this(new TopazConfiguration(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopazConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public TopazConfiguration(TopazConfiguration oldValue, TopazConfiguration newValue)
        : base(oldValue, newValue)
    {
        LogLevel = BuildConfiguration.Combine(oldValue.LogLevel, newValue.LogLevel);
        EnableLoggingToFile = BuildConfiguration.Combine(oldValue.EnableLoggingToFile, newValue.EnableLoggingToFile);
        RefreshLog = BuildConfiguration.Combine(oldValue.RefreshLog, newValue.RefreshLog);
        DefaultSubscription = BuildConfiguration.Combine(oldValue.DefaultSubscription, newValue.DefaultSubscription);
        EmulatorIpAddress = BuildConfiguration.Combine(oldValue.EmulatorIpAddress, newValue.EmulatorIpAddress);
    }
}
