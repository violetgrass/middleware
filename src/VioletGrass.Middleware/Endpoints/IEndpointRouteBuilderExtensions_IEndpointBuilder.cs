using VioletGrass.Middleware.Endpoints;

namespace VioletGrass.Middleware
{
    public static partial class IEndpointRouteBuilderExtensions
    {
        public static IEndpointBuilder<TContext> Map<TContext>(this IEndpointRouteBuilder<TContext> self) where TContext : Context
        {
            var endpointBuilder = new DefaultEndpointBuilder<TContext>(self.ServiceProvider);

            self.Map(endpointBuilder);

            return endpointBuilder;
        }
    }
}