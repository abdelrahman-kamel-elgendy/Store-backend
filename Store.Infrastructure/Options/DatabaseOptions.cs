using System.ComponentModel.DataAnnotations;

namespace Store.Infrastructure.Options;

/// <summary>SQL Server connection used by <c>AppDbContext</c>.</summary>
public sealed class DatabaseOptions
{
    /// <summary>Configuration key of the connection string.</summary>
    public const string ConnectionStringName = "StoreDb";

    /// <summary>ADO.NET connection string. Supplied by <c>ConnectionStrings__StoreDb</c>.</summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}
