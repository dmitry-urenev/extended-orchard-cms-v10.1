using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.Alias.Implementation;
using Orchard.Alias.Implementation.Holder;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Mvc.Routes;
using Orchard.StaticPages.Routing;

namespace Orchard.StaticPages
{
    public class Routes : IRouteProvider {
        private readonly ShellBlueprint _blueprint;
        private readonly IRouteHolder _routeHolder;

        public Routes(ShellBlueprint blueprint, IRouteHolder routeHolder)
        {
            _blueprint = blueprint;
            _routeHolder = routeHolder;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (RouteDescriptor routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var distinctAreaNames = _blueprint.Controllers
                .Select(controllerBlueprint => controllerBlueprint.AreaName)
                .Distinct();

            return distinctAreaNames.Select(areaName =>
                new RouteDescriptor
                {
                    Priority = 85,
                    Route = new StaticRoute(_routeHolder, areaName, new MvcRouteHandler())
                }).ToList();
        }
    }
}