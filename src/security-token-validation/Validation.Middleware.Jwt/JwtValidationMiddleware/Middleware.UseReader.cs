using System;
using System.IdentityModel.Tokens.Jwt;
using GarageGroup.Infra;
using Microsoft.OpenApi;

namespace Microsoft.AspNetCore.Builder;

partial class JwtValidationMiddleware
{
    public static TApplicationBuilder UseJwtReader<TApplicationBuilder>(
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
            appBuilder.Use(next => context => context.InvokeJwtValidationAsync(next, GetJwtReaderApi, jwtValidationStatusCodes));

        void InnerConfigureSwagger(OpenApiDocument openApiDocument)
            =>
            JwtValidationSwaggerConfigurator.Configure(openApiDocument, jwtValidationStatusCodes);

        static ISecurityTokenValidatationApi<JwtSecurityToken> GetJwtReaderApi(IServiceProvider _)
            =>
            JwtSecurityTokenReaderApi.Instance;
    }
}