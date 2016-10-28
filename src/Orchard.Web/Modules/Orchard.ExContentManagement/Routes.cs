using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using Orchard.Environment.Extensions;

namespace Orchard.ExContentManagement
{
    //[OrchardFeature("Orchard.ExContentManagement.Routing")]
    //public class Routes : IRouteProvider
    //{
    //    public void GetRoutes(ICollection<RouteDescriptor> routes)
    //    {
    //        foreach (var routeDescriptor in GetRoutes())
    //            routes.Add(routeDescriptor);
    //    }

    //    public IEnumerable<RouteDescriptor> GetRoutes()
    //    {
    //        return new[] {
    //                         new RouteDescriptor {
    //                                                 Priority = -99,
    //                                                 Route = new Route(
    //                                                     "{*path}",
    //                                                     new RouteValueDictionary {
    //                                                                                  {"area", "Orchard.ExContentManagement"},
    //                                                                                  {"controller", "Rewrite"},
    //                                                                                  {"action", "Index"}
    //                                                                              },
    //                                                     new RouteValueDictionary(),
    //                                                     new RouteValueDictionary {
    //                                                                                  {"area", "Orchard.ExContentManagement"}
    //                                                                              },
    //                                                     new MvcRouteHandler())
    //                                             }
    //                     };
    //    }
    //}
}