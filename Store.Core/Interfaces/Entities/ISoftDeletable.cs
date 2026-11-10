namespace Store.Core.Interfaces.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
    void SoftDelete();
    void Restore();
}
