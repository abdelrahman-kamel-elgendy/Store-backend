using System;

namespace Store.Core.DTOs;

/// <summary>Minimum criteria every list endpoint supports.</summary>
public interface IFilter
{
    bool IncludeDeleted { get; }
}
