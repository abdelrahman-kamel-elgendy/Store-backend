using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Core.Entities;

/// <summary>A product offered in one category.</summary>
public class Product : BaseEntity<int>
{
    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Product description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Current selling price.</summary>
    public decimal NewPrice { get; set; }

    /// <summary>Previous price.</summary>
    public decimal OldPrice { get; set; }

    /// <summary>Images stored for this product.</summary>
    public ICollection<Image> Images { get; set; } = new HashSet<Image>();

    /// <summary>Owning category id.</summary>
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }
}
