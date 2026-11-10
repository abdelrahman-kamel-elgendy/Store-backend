namespace Store.Core.Entities;

public class Photo : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public int ProductId { get; set; }
}
