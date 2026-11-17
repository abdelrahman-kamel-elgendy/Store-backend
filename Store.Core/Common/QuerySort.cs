namespace Store.Core.Common;

/// <summary>Sort keys accepted by catalog list endpoints.</summary>
public static class QuerySort
{
    /// <summary>Ascending identifier.</summary>
    public const string Id = "id";

    /// <summary>Descending identifier.</summary>
    public const string IdDesc = "-id";

    /// <summary>Ascending name.</summary>
    public const string Name = "name";

    /// <summary>Descending name.</summary>
    public const string NameDesc = "-name";

    /// <summary>Ascending creation time.</summary>
    public const string CreatedAt = "createdAt";

    /// <summary>Descending creation time.</summary>
    public const string CreatedAtDesc = "-createdAt";

    /// <summary>Ascending current price. Products only.</summary>
    public const string NewPrice = "newPrice";

    /// <summary>Descending current price. Products only.</summary>
    public const string NewPriceDesc = "-newPrice";

    /// <summary>Ascending previous price. Products only.</summary>
    public const string OldPrice = "oldPrice";

    /// <summary>Descending previous price. Products only.</summary>
    public const string OldPriceDesc = "-oldPrice";

    /// <summary>Sort keys valid for categories and products.</summary>
    public static readonly string[] Common = [Id, IdDesc, Name, NameDesc, CreatedAt, CreatedAtDesc];

    /// <summary>Sort keys valid for products, including price.</summary>
    public static readonly string[] Products = [Id, IdDesc, Name, NameDesc, CreatedAt, CreatedAtDesc, NewPrice, NewPriceDesc, OldPrice, OldPriceDesc];
}
