using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Orchard.SEO
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {   Priority = 5,
                                        Route = new Route(
                                            "robots.txt",
                                            new RouteValueDictionary {
                                                                        {"area", "Orchard.SEO"},
                                                                        {"controller", "Robots"},
                                                                        {"action", "Index"}
                                            },
                                            new RouteValueDictionary(),
                                            new RouteValueDictionary {
                                                                        {"area", "Orchard.SEO"}
                                            },
                                            new MvcRouteHandler())
                },
                new RouteDescriptor {   Priority = 5,
                                        Route = new Route(
                                            "sitemap.xml",
                                            new RouteValueDictionary {
                                                                        {"area", "Orchard.SEO"},
                                                                        {"controller", "Sitemap"},
                                                                        {"action", "Index"}
                                            },
                                            new RouteValueDictionary(),
                                            new RouteValueDictionary {
                                                                        {"area", "Orchard.SEO"}
                                            },
                                            new MvcRouteHandler())
                },
            };
        }
    }
}