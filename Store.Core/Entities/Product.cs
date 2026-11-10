using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Core.Entities;

public class Product : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();

    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }
}
