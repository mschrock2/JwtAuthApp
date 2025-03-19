using System.Net;

namespace JwtAuthApp.Helpers
{
    public class ApiResponse<T>
    {
        public T? data { get; set; } = default!;
        public string? error { get; set; } // Made nullable to fix CS8618
        public HttpStatusCode status { get; set; }
    }
}
