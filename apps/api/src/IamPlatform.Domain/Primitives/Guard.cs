namespace IamPlatform.Domain.Primitives;

public static class Guard
{
    // Método Required para strings (con mensaje personalizado)
    public static string Required(string? value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, paramName);
        }
        return value.Trim();
    }

    // Overload sin mensaje (usa mensaje por defecto)
    public static string Required(string? value, string paramName)
    {
        return Required(value, paramName, $"{paramName} is required.");
    }
}
