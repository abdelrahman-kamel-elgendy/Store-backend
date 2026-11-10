using Store.Core.Interfaces.Entities;

namespace Store.Core.Entities;

/// <summary>Provides the common persistence keys and audit metadata shared across persisted entities.</summary>
/// <typeparam name="T">The entity key type (for example <see cref="int"/> or <see cref="Guid"/>).</typeparam>
public abstract class BaseEntity<T> : IAuditable, ISoftDeletable
    where T : notnull, IEquatable<T>
{
    public T Id { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public virtual void SoftDelete()
    {
        IsDeleted = true;
    }

    public virtual void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
