namespace Store.Core.Entities;

public class Image : BaseEntity<int>
{
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public int ProductId { get; set; }
}
