using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Modularity;

namespace saasLMS.Shared.Hosting.AspNetCore;

public static class SwaggerConfigurationHelper
{
    public static void Configure(
        ServiceConfigurationContext context,
        string apiTitle
    )
    {
        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = apiTitle,
                Version = "v1"
            });

            options.DocInclusionPredicate((_, _) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }

    public static void ConfigureWithOidc(
        ServiceConfigurationContext context,
        string authority,
        string[] scopes,
        string apiTitle,
        string apiVersion = "v1",
        string apiName = "v1",
        string[]? flows = null,
        string? discoveryEndpoint = null
    )
    {
        var env = context.Services.GetHostingEnvironment();
        var configuredFlows = flows ?? new[] { "authorization_code" };

        if (env.IsDevelopment())
        {
            var scopeDictionary = scopes.ToDictionary(scope => scope, scope => scope);
            context.Services.AddAbpSwaggerGenWithOAuth(
                authority,
                scopes: scopeDictionary,
                options =>
                {
                    options.SwaggerDoc(apiName, new OpenApiInfo
                    {
                        Title = apiTitle,
                        Version = apiVersion
                    });

                    options.DocInclusionPredicate((_, _) => true);
                    options.CustomSchemaIds(type => type.FullName);
                });

            return;
        }

        context.Services.AddAbpSwaggerGenWithOidc(
            authority,
            scopes: scopes,
            flows: configuredFlows,
            discoveryEndpoint: discoveryEndpoint,
            options =>
            {
                options.SwaggerDoc(apiName, new OpenApiInfo
                {
                    Title = apiTitle,
                    Version = apiVersion
                });

                options.DocInclusionPredicate((_, _) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }
}
