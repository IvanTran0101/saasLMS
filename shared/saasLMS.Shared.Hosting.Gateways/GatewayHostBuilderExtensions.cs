using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

public static class AbpHostingHostBuilderExtensions
{
    public const string AppYarpJsonPath = "yarp.json";

    public static IHostBuilder AddYarpJson(
        this IHostBuilder hostBuilder,
        bool optional = true,
        bool reloadOnChange = true,
        string path = AppYarpJsonPath)
    {
        return hostBuilder.ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddJsonFile(
                path: path,
                optional: optional,
                reloadOnChange: reloadOnChange
            ); 
            
            var yarpJsonPath = path.Replace(".json", "");
            builder.AddJsonFile(
                path: $"{yarpJsonPath}.{context.HostingEnvironment.EnvironmentName}.json",
                optional: optional,
                reloadOnChange: reloadOnChange
            );

            // Ensure container environment variables can override yarp.json (Swarm/K8s).
            // WebApplication.CreateBuilder(args) adds env vars early; AddYarpJson runs later and would
            // otherwise override those values.
            builder.AddEnvironmentVariables();
        });
    }
}
