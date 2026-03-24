namespace IamPlatform.Domain.Primitives;

public static class Guard
{
    public static string Required(string? value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, paramName);
        }
        return value.Trim();
    }

    public static string Required(string? value, string paramName)
    {
        return Required(value, paramName, $"{paramName} is required.");
    }
}
