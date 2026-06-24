namespace Testcontainers.Topaz;

/// <inheritdoc cref="ContainerConfiguration" />
public sealed class TopazConfiguration : ContainerConfiguration
{
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
    }
}
