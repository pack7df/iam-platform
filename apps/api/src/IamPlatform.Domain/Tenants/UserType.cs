namespace IamPlatform.Domain.Tenants;

public enum UserType
{
    SystemUser = 0,
    TenantAdmin = 1,
    EndUser = 2,
    ServiceAdmin = 3,
}
