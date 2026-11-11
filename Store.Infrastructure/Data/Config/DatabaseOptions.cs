using System.ComponentModel.DataAnnotations;

namespace Store.API.Configuration;

public sealed class DatabaseOptions
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}
