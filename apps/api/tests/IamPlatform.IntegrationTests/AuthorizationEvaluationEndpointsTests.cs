using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IamPlatform.IntegrationTests;

public sealed class AuthorizationEvaluationEndpointsTests
{
    private async Task<(string UserId, string ParentResId, string ChildResId, string OpId)> SeedScenario(ApiWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IamPlatformDbContext>();

        var tenantId = Guid.NewGuid().ToString();
        var appId = Guid.NewGuid().ToString();
        var parentResId = Guid.NewGuid().ToString();
        var childResId = Guid.NewGuid().ToString();
        var opId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();

        var tenant = Tenant.Create(tenantId, "Eval Tenant");
        var app = IamPlatform.Domain.Tenants.Application.Create(appId, tenantId, "Eval App");
        var parentRes = Resource.CreateRoot(parentResId, appId, "Parent", "parent");
        var childRes = Resource.CreateChild(childResId, appId, "Child", "child", parentResId);
        var op = Operation.Create(opId, appId, "read", "Read");
        var user = User.Create(userId, tenantId, UserType.EndUser);

        // Rule: Allow user on Parent Resource
        var parentRule = AuthorizationRule.CreateForUser(Guid.NewGuid().ToString(), user, parentRes, op, AuthorizationRuleDecision.Allow);
        // Rule: Inherit on Child Resource
        var childRule = AuthorizationRule.CreateForUser(Guid.NewGuid().ToString(), user, childRes, op, AuthorizationRuleDecision.Inherit);

        context.Tenants.Add(tenant);
        context.Applications.Add(app);
        context.Resources.AddRange(parentRes, childRes);
        context.Operations.Add(op);
        context.Users.Add(user);
        context.AuthorizationRules.AddRange(parentRule, childRule);
        
        await context.SaveChangesAsync();

        return (userId, parentResId, childResId, opId);
    }

    [Fact]
    public async Task Evaluate_Should_Return_Allow_For_Direct_Rule()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (userId, parentResId, _, opId) = await SeedScenario(factory);

        var request = new { userId, resourceId = parentResId, operationId = opId };
        var response = await client.PostAsJsonAsync("/api/authorization/evaluate", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<EvaluationResponse>();
        result.Should().NotBeNull();
        result!.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be("Allow");
    }

    [Fact]
    public async Task Evaluate_Should_Return_Allow_For_Inherited_Rule()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (userId, _, childResId, opId) = await SeedScenario(factory);

        var request = new { userId, resourceId = childResId, operationId = opId };
        var response = await client.PostAsJsonAsync("/api/authorization/evaluate", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<EvaluationResponse>();
        result.Should().NotBeNull();
        result!.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be("Allow");
    }

    [Fact]
    public async Task Evaluate_Should_Return_Deny_When_No_Rule_Matches()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (_, parentResId, _, opId) = await SeedScenario(factory);
        var otherUserId = Guid.NewGuid().ToString(); // Not seeded with rules

        var request = new { userId = otherUserId, resourceId = parentResId, operationId = opId };
        var response = await client.PostAsJsonAsync("/api/authorization/evaluate", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<EvaluationResponse>();
        result.Should().NotBeNull();
        result!.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be("Deny");
    }

    private record EvaluationResponse(bool IsAuthorized, string Decision, string? ResolvedResourceId, List<string> AppliedRuleIds);
}
