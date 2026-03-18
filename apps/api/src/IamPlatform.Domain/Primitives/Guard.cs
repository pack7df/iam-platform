namespace IamPlatform.Domain.Primitives;

internal static class Guard
{
    public static string Required(string? value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, paramName);
        }

        return value.Trim();
    }
}
