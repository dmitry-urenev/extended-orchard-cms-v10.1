using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Orchard.CultureSwitcher
{
    public class Routes : IRouteProvider
    {
        #region IRouteProvider Members

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (RouteDescriptor routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Name = "Homepage",
                    Priority = 85,
                    Route = new Route(
                        "",
                        new RouteValueDictionary {
                            {"area", "Orchard.Localization"},
                            {"controller", "LocalizableHome"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary {
                            {"area", "Orchard.Localization"},
                            {"controller", "LocalizableHome"},
                        },
                        new RouteValueDictionary {
                            {"area", "Orchard.Localization"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        #endregion
    }
}