using Microsoft.AspNetCore.Http;

namespace Store.Core.Interfaces.Services;

public interface IImageManagementService
{
    Task<List<string>> AddImageAsync(IFormFileCollection files, string src);
    Task DeleteImageAsync(string src);
}
