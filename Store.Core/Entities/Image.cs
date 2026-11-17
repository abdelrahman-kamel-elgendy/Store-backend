namespace Store.Core.Entities;

/// <summary>A file stored for a product. The entity name is Image; the table is Images.</summary>
public class Image : BaseEntity<int>
{
    /// <summary>Relative path under the web root, using forward slashes.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>True when this is the primary image.</summary>
    public bool IsMain { get; set; }

    /// <summary>Owning product id.</summary>
    public int ProductId { get; set; }
}
