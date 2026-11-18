using Store.API.DTO;
using Store.API.Helper;

namespace Store.API.Services;

public interface IApiResponseBuilder
{
    ApiResponse<object> BuildExceptionResponse(Exception exception, string traceId);
    ApiResponse<T> BuildSuccessResponse<T>(T data, string message, ApiMetadata? meta = null);
    ApiResponse<object> BuildStatusCodeResponse(int statusCode, string traceId);
}