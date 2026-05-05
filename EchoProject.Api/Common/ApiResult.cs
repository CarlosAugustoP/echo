namespace EchoProject.Api.Common
{
    public class ApiResult<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }
        public string? ErrorCode { get; private set; }
        public string? StackTrace { get; private set; } = string.Empty;
        
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        public static ApiResult<T> Ok(T data) => new() { Success = true, Data = data };
        public static ApiResult<T> Failure(string error, string errorCode, string? stackTrace = null) => new() { Success = false, Error = error, ErrorCode = errorCode, StackTrace = stackTrace ?? string.Empty };
    }
}