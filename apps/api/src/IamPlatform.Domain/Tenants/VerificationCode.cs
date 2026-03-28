namespace IamPlatform.Domain.Tenants;

public enum VerificationCodeType
{
    Registration = 1,
    PasswordReset = 2,
    EmailChange = 3
}

public sealed class VerificationCode
{
    private VerificationCode(string id, string userId, string code, DateTime expiresAt, VerificationCodeType type)
    {
        Id = id;
        UserId = userId;
        Code = code;
        ExpiresAt = expiresAt;
        Type = type;
        IsUsed = false;
    }

    public string Id { get; }
    public string UserId { get; }
    public string Code { get; }
    public DateTime ExpiresAt { get; }
    public VerificationCodeType Type { get; }
    public bool IsUsed { get; private set; }

    public bool IsExpired(DateTime now) => now > ExpiresAt;

    public static VerificationCode Create(string userId, string code, DateTime expiresAt, VerificationCodeType type)
    {
        return new VerificationCode(Guid.NewGuid().ToString(), userId, code, expiresAt, type);
    }

    public void Use()
    {
        if (IsUsed) throw new InvalidOperationException("Code already used.");
        IsUsed = true;
    }
}
