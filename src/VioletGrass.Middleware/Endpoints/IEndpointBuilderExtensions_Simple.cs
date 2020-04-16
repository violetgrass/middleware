namespace VioletGrass.Middleware
{
    public static partial class IEndpointBuilderExtensions
    {
        public static IEndpointBuilder<TContext> WithDisplayName<TContext>(this IEndpointBuilder<TContext> self, string displayName) where TContext : Context
        {
            self.DisplayName = displayName;

            return self;
        }
        public static IEndpointBuilder<TContext> WithMiddlewareDelegate<TContext>(this IEndpointBuilder<TContext> self, MiddlewareDelegate<TContext> @delegate) where TContext : Context
        {
            self.MiddlewareDelegate = @delegate;

            return self;
        }
    }
}