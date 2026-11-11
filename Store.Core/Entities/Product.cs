using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Core.Entities;

public class Product : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal NewPrice { get; set; }
    public decimal OldPrice { get; set; }
    public ICollection<Image> Images { get; set; } = new HashSet<Image>();

    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }
}
