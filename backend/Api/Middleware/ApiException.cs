namespace Api.Middleware;

/// <summary>
/// Excepción controlada para devolver errores HTTP con mensaje amigable en ApiResponse.
/// </summary>
public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
