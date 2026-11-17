using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Core.Entities;

/// <summary>A catalog category that can contain child categories and products.</summary>
public class Category : BaseEntity<int>
{
    /// <summary>Display name. Unique among active categories is enforced by the service, not by a unique index.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description shown with the category.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Parent category id, or null for a root.</summary>
    public int? ParentCategoryId { get; set; }

    [ForeignKey(nameof(ParentCategoryId))]
    public virtual Category? ParentCategory { get; set; }

    [InverseProperty(nameof(ParentCategory))]
    public virtual ICollection<Category> SubCategories { get; set; } = [];


    public virtual ICollection<Product> Products { get; set; } = [];
}