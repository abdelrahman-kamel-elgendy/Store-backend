using System.ComponentModel.DataAnnotations;

namespace Store.API.Configuration;

public sealed class DatabaseOptions
{
    [Required(ErrorMessage = "Connection string is required.")]
    public string ConnectionString { get; set; } = string.Empty;
}
