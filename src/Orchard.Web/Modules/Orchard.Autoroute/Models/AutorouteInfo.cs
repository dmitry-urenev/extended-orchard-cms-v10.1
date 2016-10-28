using Orchard.ContentManagement;
using System.Web.Routing;

namespace Orchard.Autoroute.Models
{
    public class AutorouteInfo
    {
        public ContentItem Content { get; set; }

        public RouteValueDictionary RouteValues { get; set; }
    }
}
