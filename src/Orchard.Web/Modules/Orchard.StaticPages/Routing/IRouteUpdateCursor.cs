
namespace Orchard.StaticPages.Routing
{
    public interface IRouteUpdateCursor : ISingletonDependency
    {
        int Cursor { get; set; }
    }
}