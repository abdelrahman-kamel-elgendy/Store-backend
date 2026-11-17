namespace Store.Core.Common;

/// <summary>Identifiers stored in audit columns when no user principal is available.</summary>
public static class AuditUsers
{
    /// <summary>Value written when the request has no authenticated user.</summary>
    public const string System = "system";
}
