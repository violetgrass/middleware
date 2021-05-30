using System;

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

        public static IEndpointBuilder<TContext> WithMetadata<TContext>(this IEndpointBuilder<TContext> self, object metadata) where TContext : Context
        {
            self.Metadata.Add(metadata);

            return self;
        }

        public static IEndpointBuilder<TContext> Requires<TContext>(this IEndpointBuilder<TContext> self, Predicate<TContext> predicate) where TContext : Context
        {
            self.Predicates.Add(predicate);

            return self;
        }

        public static IEndpointBuilder<TContext> Requires<TContext>(this IEndpointBuilder<TContext> self, Predicate<TContext>[] predicates) where TContext : Context
        {
            foreach (var p in predicates)
            {
                self.Predicates.Add(p);
            }

            return self;
        }
    }
}