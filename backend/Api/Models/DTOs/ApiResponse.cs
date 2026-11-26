namespace Api.Models.DTOs;

/// <summary>
/// Formato estándar de respuesta para todos los endpoints de la API.
/// </summary>
public class ApiResponse<T>
{
    public bool Ok { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }

    public static ApiResponse<T> Success(T data) => new()
    {
        Ok = true,
        Data = data,
        Error = null
    };

    public static ApiResponse<T> Fail(string error) => new()
    {
        Ok = false,
        Data = default,
        Error = error
    };
}
