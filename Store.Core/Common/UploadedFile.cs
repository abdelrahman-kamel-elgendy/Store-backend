namespace Store.Core.Common;

/// <summary>A file received from the caller, without an ASP.NET Core type.</summary>
public sealed class UploadedFile
{
    /// <summary>Original file name supplied by the client. Used only to read the extension.</summary>
    public required string FileName { get; init; }

    /// <summary>Declared media type, for example <c>image/jpeg</c>.</summary>
    public required string ContentType { get; init; }

    /// <summary>Size in bytes.</summary>
    public required long Length { get; init; }

    /// <summary>Readable content. The image service disposes this stream after copying it.</summary>
    public required Stream Content { get; init; }
}
