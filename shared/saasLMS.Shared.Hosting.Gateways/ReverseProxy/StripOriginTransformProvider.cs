using System.Threading.Tasks;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace saasLMS.Shared.Hosting.Gateways.ReverseProxy;

/// <summary>
/// In a gateway architecture, browser CORS is handled at the gateway boundary.
/// Forwarding the browser's Origin/preflight headers downstream causes noisy logs
/// and can accidentally apply downstream CORS policies to proxied calls.
/// </summary>
public sealed class StripOriginTransformProvider : ITransformProvider
{
    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(transformContext =>
        {
            transformContext.ProxyRequest.Headers.Remove("Origin");
            transformContext.ProxyRequest.Headers.Remove("Access-Control-Request-Method");
            transformContext.ProxyRequest.Headers.Remove("Access-Control-Request-Headers");
            return ValueTask.CompletedTask;
        });
    }

    public void ValidateRoute(TransformRouteValidationContext context)
    {
        // No route-specific configuration to validate.
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
        // No cluster-specific configuration to validate.
    }
}
