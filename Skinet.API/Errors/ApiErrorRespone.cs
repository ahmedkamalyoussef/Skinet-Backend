namespace Skinet.API.Errors;

public class ApiErrorRespone(int code, string message, string? details)
{
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;
    public string? Details { get; set; } = details;
}