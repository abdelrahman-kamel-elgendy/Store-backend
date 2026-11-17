using Store.Core.Interfaces.Entities;

namespace Store.Core.Entities;

/// <summary>Provides the common persistence keys and audit metadata shared across persisted entities.</summary>
/// <typeparam name="T">The entity key type (for example <see cref="int"/> or <see cref="Guid"/>).</typeparam>
public abstract class BaseEntity<T> : IAuditable, ISoftDeletable
    where T : notnull, IEquatable<T>
{
    /// <summary>Primary key.</summary>
    public T Id { get; set; } = default!;

    /// <summary>UTC creation time.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC last-update time.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>User who created the row, or <c>system</c>.</summary>
    public string? CreatedBy { get; set; }

    /// <summary>User who last updated the row.</summary>
    public string? UpdatedBy { get; set; }

    /// <summary>True when the row is soft-deleted.</summary>
    public bool IsDeleted { get; set; }

    /// <summary>UTC deletion time.</summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>User who soft-deleted the row.</summary>
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
