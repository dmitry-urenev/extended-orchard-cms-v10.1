using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Routing;
using Orchard.Alias.Implementation.Holder;
using System.Collections.Concurrent;
using System.Web.Mvc;
using System.Web;

namespace Orchard.StaticPages.Routing
{
    public class RouteMap
    {
        private readonly string _area;
        private readonly List<RouteInfo> _routes;

        public RouteMap(string area)
        {
            _area = area;
            _routes = new List<RouteInfo>();
        }

        public IEnumerable<RouteInfo> GetRoutes()
        {
            return _routes.AsEnumerable();
        }    

        /// <summary>
        /// Adds an <see cref="AliasInfo"/> to the map
        /// </summary>
        /// <param name="info">The <see cref="AliasInfo"/> intance to add</param>
        public void Insert(AliasInfoEx info)
        {
            if (info == null)
                throw new ArgumentNullException();

            var routeValues = info.RouteValues.ToDictionary(kv => kv.Key, kv => (object)kv.Value);
            foreach (string key in routeValues.Keys.ToArray())
            {
                var v = routeValues[key];
                if (string.IsNullOrEmpty((string)v))
                {
                    routeValues[key] = UrlParameter.Optional;
                }
            }

            if (!_routes.Any(r => r.Id == info.Id))
            {
                RouteInfo routeInfo = new RouteInfo
                {
                    Id = info.Id,
                    Route = new Route(info.Path,
                    new RouteValueDictionary(routeValues),
                    null,
                    new RouteValueDictionary() { { "area", info.Area } },
                    null)
                };

                _routes.Add(routeInfo);
            }
        }

        private bool Contains(string path)
        {
            return _routes.Any(r => r.Route.Url.Equals(path, StringComparison.OrdinalIgnoreCase));
        }

        public void Remove(string path)
        {
            var routes = _routes.Where(r => r.Route.Url.Equals(path, StringComparison.OrdinalIgnoreCase)).ToList();
            routes.ForEach(r => _routes.Remove(r));
        }

        public void Remove(AliasInfoEx info)
        {
            var routes = _routes.Where(r => r.Id == info.Id).ToList();
            routes.ForEach(r => _routes.Remove(r));
        }

        public bool Any()
        {
            return _routes.Any();
        }
    }
}