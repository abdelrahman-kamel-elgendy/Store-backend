namespace Store.Infrastructure.Options;

/// <summary>Opt-in switch for development sample data.</summary>
public sealed class SeedDatabaseOptions
{
    /// <summary>Environment variable and configuration key.</summary>
    public const string ConfigurationKey = "SEED_DATABASE";

    /// <summary>When true, sample categories and products are inserted at startup. Allowed only in Development.</summary>
    public bool Enabled { get; set; }
}
