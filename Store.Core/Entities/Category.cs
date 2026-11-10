using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Core.Entities;

public class Category : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;


    public int? ParentCategoryId { get; set; }

    [ForeignKey(nameof(ParentCategoryId))]
    public virtual Category? ParentCategory { get; set; }

    [InverseProperty(nameof(ParentCategory))]
    public virtual ICollection<Category> SubCategories { get; set; } = [];


    public virtual ICollection<Product> Products { get; set; } = [];
}