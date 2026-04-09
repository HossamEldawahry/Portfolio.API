using Microsoft.AspNetCore.Authorization;

namespace Portfolio.API.Authorization;

public sealed class AdminApiKeyAuthorizationHandler : AuthorizationHandler<AdminApiKeyRequirement>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminApiKeyAuthorizationHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminApiKeyRequirement requirement)
    {
        if (context.User.IsInRole(AppRoles.Admin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var configuredKey = _configuration["Portfolio:AdminApiKey"];
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            return Task.CompletedTask;
        }

        var http = _httpContextAccessor.HttpContext;
        if (http is null)
            return Task.CompletedTask;

        if (http.Request.Headers.TryGetValue("X-Api-Key", out var headerValue))
        {
            var provided = headerValue.ToString();
            if (string.Equals(provided, configuredKey, StringComparison.Ordinal))
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
