using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.Localization.Services;
using System.Collections.Generic;
using System;

namespace Orchard.StaticPages.Routing
{
    public class StaticRoute : RouteBase, IRouteWithArea {

        private readonly RouteMap _routeMap;
        private readonly IRouteHolder _routeHolder;
        private readonly IRouteHandler _routeHandler;       

        public StaticRoute(IRouteHolder routeHolder, string areaName, IRouteHandler routeHandler)
        {
            Area = areaName;
            _routeHolder = routeHolder;
            _routeMap = routeHolder.GetMap(areaName);            
            _routeHandler = routeHandler;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            foreach (var r in _routeMap.GetRoutes())
            {
                var data = r.Route.GetRouteData(httpContext);
                if (data != null)
                {
                    data.RouteHandler = _routeHandler;
                    data.DataTokens["_staticUrl"] = r.Route.Url;
                    return data;
                }                    
            }

            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues) {

            var routes = _routeMap.GetRoutes();
            var workContext = requestContext.GetWorkContext();
            if (workContext != null)
            {
                var cultureManager = workContext.Resolve<ICultureManager>();
                var cultures = cultureManager.ListCultures();
                var defaultCulture = cultureManager.GetSiteCulture();
                var currentCulture = cultureManager.GetCurrentCulture(requestContext.HttpContext);

                var targetRoutes = routes.Where(r => r.Route.Url.StartsWith(currentCulture, StringComparison.OrdinalIgnoreCase)).ToList();
                if (!targetRoutes.Any())
                {
                    targetRoutes = routes.Where(r => !cultures.Any(c => r.Route.Url.StartsWith(c, StringComparison.OrdinalIgnoreCase))).ToList();
                }
                routes = targetRoutes;
            }
            foreach (var r in routes)
            {
                var pathData = r.Route.GetVirtualPath(requestContext, routeValues);
                if (pathData != null)
                    return pathData;
            }
            if ("Orchard.StaticPages".Equals(Area))
            {
                if (routeValues.ContainsKey("Controller") && "Page".Equals(routeValues["Controller"]))
                {
                    if (routeValues.ContainsKey("Action") && "Display".Equals(routeValues["Action"]))
                    {
                        string path = (string)routeValues["path"];
                        var rInfo = _routeHolder.GetMaps().SelectMany(m => m.GetRoutes(), (map, ri) => ri)
                            .Where(ri => ri.Route.Url == path)
                            .FirstOrDefault();

                        if (rInfo != null) {
                            var defaultRouteValues = rInfo.Route.Defaults;
                            return rInfo.Route.GetVirtualPath(requestContext, defaultRouteValues);
                        }
                    }
                }
            }

            return null;
        }

        public string Area { get; private set; }

        private class LocalizedPathComparer : IComparer<string>
        {
            private IEnumerable<string> cultures;
            private string defaultCulture;
            private string _culture;

            private LocalizedPathComparer(string culture)
            {
                _culture = culture;
            }

            private LocalizedPathComparer(string culture, IEnumerable<string> cultures) : this(culture)
            {
                this.cultures = cultures;
            }

            public LocalizedPathComparer(string culture, string defaultCulture, IEnumerable<string> cultures) : this(culture)
            {
                this.defaultCulture = defaultCulture;
                this.cultures = cultures;
            }

            public int Compare(string x, string y)
            {
                if (x != null && y != null)
                {
                    int idx_x = x.IndexOf(_culture, StringComparison.OrdinalIgnoreCase);
                    int idx_y = y.IndexOf(_culture, StringComparison.OrdinalIgnoreCase);

                    if (defaultCulture == _culture)
                    {

                    }

                    return idx_y - idx_x;
                }
                else if (x == null && y != null)
                {
                    return 1;
                }
                else if (x != null && y == null)
                {
                    return -1;
                }
                else
                    return 0;
            } 
        }
    }
}