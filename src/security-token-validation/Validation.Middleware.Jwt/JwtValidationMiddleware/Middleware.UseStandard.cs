using System;
using System.IdentityModel.Tokens.Jwt;
using GarageGroup.Infra;
using Microsoft.OpenApi;
using PrimeFuncPack;

namespace Microsoft.AspNetCore.Builder;

partial class JwtValidationMiddleware
{
    public static TApplicationBuilder UseJwtValidation<TApplicationBuilder>(
        this TApplicationBuilder appBuilder, JwtValidationStatusCodes? jwtValidationStatusCodes = null)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(appBuilder);

        if (jwtValidationStatusCodes is not null && jwtValidationStatusCodes.Value.NotSpecifiedHeaderValueStatusCode is null)
        {
            _ = appBuilder.UseWhen(IsAuthorizationHeaderSpecified, InnerConfigureMiddleware);
        }
        else
        {
            InnerConfigureMiddleware(appBuilder);
        }

        if (appBuilder is ISwaggerBuilder swaggerBuilder)
        {
            _ = swaggerBuilder.Use(InnerConfigureSwagger);
        }

        return appBuilder;

        void InnerConfigureMiddleware(IApplicationBuilder app)
            =>
            appBuilder.Use(next => context => context.InvokeJwtValidationAsync(next, ResolveStandardValidationApi, jwtValidationStatusCodes));

        void InnerConfigureSwagger(OpenApiDocument openApiDocument)
            =>
            JwtValidationSwaggerConfigurator.Configure(openApiDocument, jwtValidationStatusCodes);
    }

    private static ISecurityTokenValidatationApi<JwtSecurityToken> ResolveStandardValidationApi(IServiceProvider serviceProvider)
        =>
        Dependency.Of<IIssuerSigningKeyApi>(new RsaSecurityKeyApi()).UseJwtSecurityTokenValidation().Resolve(serviceProvider);
}