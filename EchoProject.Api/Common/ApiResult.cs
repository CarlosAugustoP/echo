namespace EchoProject.Api.Common
{
    public class ApiResult<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public List<string> Errors { get; private set; } = [];
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        public static ApiResult<T> Ok(T data) => new() { Success = true, Data = data };
        public static ApiResult<T> Failure(IEnumerable<string> errors) => new() { Success = false, Errors = errors.ToList() };
        public static ApiResult<T> Failure(string error) => new() { Success = false, Errors = [error] };
    }
}