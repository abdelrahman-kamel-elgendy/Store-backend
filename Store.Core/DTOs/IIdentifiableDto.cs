namespace Store.Core.DTOs;

/// <summary>Identifies a resource response by its integer id.</summary>
public interface IIdentifiableDto
{
    /// <summary>Database identifier of the resource.</summary>
    int Id { get; }
}
