namespace VioletGrass.Middleware
{
    public interface IRouteContextAware<TContext> where TContext : Context
    {
        void PushRouteContext(Route<TContext> isApplicable);
        void PopRouteContext();
    }
}