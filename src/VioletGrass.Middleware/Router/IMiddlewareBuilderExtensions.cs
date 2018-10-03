using VioletGrass.Middleware.Router;

namespace VioletGrass.Middleware
{
    public static class IMiddlewareBuilderExtensions_Router
    {
        public static IMiddlewareBuilder UseRoutes(this IMiddlewareBuilder self, params Route[] routes)
        {
            if (self == null)
            {
                throw new System.ArgumentNullException(nameof(self));
            }

            if (routes == null)
            {
                throw new System.ArgumentNullException(nameof(routes));
            }

            foreach (var route in routes)
            {
                if (route.IsApplicable == null)
                {
                    throw new System.ArgumentNullException(nameof(route.IsApplicable));
                }

                if (route.MiddlewareBuilderForRoute == null)
                {
                    throw new System.ArgumentNullException(nameof(route.MiddlewareBuilderForRoute));
                }
            }

            return self.Use(InternalRouter.CreateMiddlewareFactory(self, routes));
        }
    }
}