using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Portfolio.API.Infrastructure;

public sealed class AuthExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var action = context.ApiDescription.RelativePath?.ToLowerInvariant() ?? string.Empty;
        if (!action.StartsWith("api/v1/auth/"))
            return;

        if (action.EndsWith("login"))
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Example = new OpenApiObject
                        {
                            ["username"] = new OpenApiString("admin"),
                            ["password"] = new OpenApiString("ChangeMe123!")
                        }
                    }
                }
            };
        }
        else if (action.EndsWith("refresh") || action.EndsWith("logout"))
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Example = new OpenApiObject
                        {
                            ["refreshToken"] = new OpenApiString("paste_refresh_token_here")
                        }
                    }
                }
            };
        }
    }
}
