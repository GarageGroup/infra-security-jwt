using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi;

namespace GarageGroup.Infra;

partial class JwtValidationSwaggerConfigurator
{
    internal static void Configure(OpenApiDocument openApiDocument, JwtValidationStatusCodes? jwtValidationStatusCodes)
    {
        if (openApiDocument is null)
        {
            return;
        }

        openApiDocument.Components ??= new();
        openApiDocument.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        openApiDocument.Components.SecuritySchemes[SecuritySchemeKey] = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT authorization header"
        };

        var referenceScurityScheme = new OpenApiSecuritySchemeReference(
            referenceId: SecuritySchemeKey,
            hostDocument: openApiDocument);

        var securityRequirement = new OpenApiSecurityRequirement
        {
            [referenceScurityScheme] = []
        };

        if (openApiDocument.Paths?.Count is not > 0)
        {
            return;
        }

        var codeDescriptions = GetCodeDescriptions(jwtValidationStatusCodes);

        foreach (var path in openApiDocument.Paths.Values.SelectMany(GetOperations))
        {
            path.Security ??= [];
            path.Security.Add(securityRequirement);

            if (codeDescriptions.Count is not > 0)
            {
                continue;
            }

            path.Responses ??= [];

            foreach (var codeDescription in codeDescriptions)
            {
                if (path.Responses.ContainsKey(codeDescription.Key))
                {
                    continue;
                }

                path.Responses[codeDescription.Key] = new OpenApiResponse
                {
                    Description = codeDescription.Value
                };
            }
        }
    }

    private static IEnumerable<OpenApiOperation> GetOperations(IOpenApiPathItem pathItem)
        =>
        pathItem.Operations?.Select(GetValue) ?? [];

    private static TValue GetValue<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
        =>
        pair.Value;
}