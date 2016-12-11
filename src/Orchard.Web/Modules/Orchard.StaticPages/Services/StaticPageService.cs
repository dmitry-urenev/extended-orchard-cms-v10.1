using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.StaticPages.Models;
using Orchard.StaticPages.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.StaticPages.Services
{
    public class StaticPageService : IStaticPageService
    {
        private readonly IRouteStorage _routeStorage;

        public StaticPageService(IRouteStorage routeStorage)
        {
            _routeStorage = routeStorage;
        }

        public void UpdateRoute(StaticPagePart part, string staticUrl)
        {
            part.IsAction = false;
            part.StaticPageUrl = staticUrl;

            _routeStorage.Remove(part.Record.RouteId);

            var path = part.As<IAliasAspect>().Path;

            part.Route = _routeStorage.Set(path, staticUrl);
        }

        public void UpdateRoute(StaticPagePart part, IDictionary<string, string> routeValues)
        {
            part.IsAction = true;
            part.StaticPageUrl = null;

            _routeStorage.Remove(part.Record.RouteId);

            var path = part.As<IAliasAspect>().Path;

            if (!routeValues.ContainsKey("area"))
            {
                throw new ArgumentNullException("StaticPagePart: \"area\" is not defined");
            }
            else if (!routeValues.ContainsKey("controller"))
            {
                throw new ArgumentNullException("StaticPagePart: \"controller\" is not defined");
            }
            else if (!routeValues.ContainsKey("action"))
            {
                throw new ArgumentNullException("StaticPagePart: \"action\" is not defined");
            }
            part.Route = _routeStorage.Set(path, routeValues);
        }

        public void Route(StaticPagePart part, string path)
        {
            if (part.Route != null)
            {
                var routeValues = _routeStorage.Get(part.Route.Path);
                _routeStorage.Remove(part.Route.Path);

                part.Route = _routeStorage.Set(path, routeValues);
            }
        }

        public void RemoveAliases(StaticPagePart part)
        {
            if (part.Route != null)
            {
                _routeStorage.Remove(part.Route.Path);
            }
        }
    }
}