using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public delegate Task MiddlewareDelegate(Context context);
}