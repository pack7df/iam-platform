namespace IamPlatform.Domain.Authorization;

public class EvaluationContext
{
    public AuthorizationRequest Request { get; }
    
    public EvaluationContext(AuthorizationRequest request)
    {
        Request = request;
    }
}

