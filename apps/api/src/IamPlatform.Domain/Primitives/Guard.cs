namespace IamPlatform.Domain.Primitives;

public static class Guard
{
    // Para cualquier tipo de referencia (objetos)
    public static T Required<T>(T value, string paramName) where T : class
    {
        if (value is null)
            throw new ArgumentNullException(paramName);
        return value;
    }

    // Para objetos con mensaje personalizado
    public static T Required<T>(T value, string paramName, string message) where T : class
    {
        if (value is null)
            throw new ArgumentNullException(paramName, message);
        return value;
    }

    // Para strings (devuelve string con Trim)
    public static string Required(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} is required.", paramName);
        return value.Trim();
    }

    // Para strings con mensaje personalizado
    public static string Required(string? value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(message, paramName);
        return value.Trim();
    }
}
