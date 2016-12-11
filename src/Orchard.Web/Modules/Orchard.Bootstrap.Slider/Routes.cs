using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Orchard.Bootstrap.Slider
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
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders/Create",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderAdmin"},
                                                                                      {"action", "Create"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders/{sliderId}/Edit",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderAdmin"},
                                                                                      {"action", "Edit"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders/{sliderId}/Remove",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderAdmin"},
                                                                                      {"action", "Remove"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders/{sliderId}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderAdmin"},
                                                                                      {"action", "Items"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders/{sliderId}/Items/Create",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderItemAdmin"},
                                                                                      {"action", "Create"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders/{sliderId}/Items/{itemId}/Edit",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderItemAdmin"},
                                                                                      {"action", "Edit"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                Route = new Route(
                                    "Admin/Sliders/{sliderId}/Items/{itemId}/Delete",
                                    new RouteValueDictionary {
                                                                {"area", "Orchard.Bootstrap.Slider"},
                                                                {"controller", "SliderItemAdmin"},
                                                                {"action", "Delete"}
                                                            },
                                    new RouteValueDictionary (),
                                    new RouteValueDictionary {
                                                                {"area", "Orchard.Bootstrap.Slider"}
                                                            },
                                    new MvcRouteHandler())
                             },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders/{sliderId}/Items/{itemId}/Publish",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderItemAdmin"},
                                                                                      {"action", "Publish"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders/{sliderId}/Items/{itemId}/Unpublish",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderItemAdmin"},
                                                                                      {"action", "Unpublish"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Sliders",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"},
                                                                                      {"controller", "SliderAdmin"},
                                                                                      {"action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Bootstrap.Slider"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
            };
        }
    }
}