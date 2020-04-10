namespace VioletGrass.Middleware
{
    public static partial class ContextExtensions
    {
        public static TFeature Feature<TFeature>(this Context self) where TFeature : class
            => self.Features.Get<TFeature>();
    }
}