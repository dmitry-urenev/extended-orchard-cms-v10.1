using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Orchard.Alias.Implementation.Map;
using System.Web.Routing;
using Orchard.Alias.Implementation.Holder;

namespace Orchard.StaticPages.Routing 
{
    public class RouteHolder : IRouteHolder 
    {
        private readonly ConcurrentDictionary<string, RouteMap> _routeMaps;

        public RouteHolder()
        {
            _routeMaps = new ConcurrentDictionary<string, RouteMap>(StringComparer.OrdinalIgnoreCase);
        }

        public void SetAliases(IEnumerable<AliasInfoEx> aliases) {
            var grouped = aliases.GroupBy(alias => alias.Area ?? String.Empty, StringComparer.InvariantCultureIgnoreCase);

            foreach (var group in grouped) {
                var map = GetMap(group.Key);

                foreach (var alias in group) {
                    map.Insert(alias);
                }
            }
        }

        public void SetAlias(AliasInfoEx alias)
        {
            foreach (var map in _routeMaps.Values)
            {
                map.Remove(alias);
            }

            GetMap(alias.Area).Insert(alias);
        }

        public IEnumerable<RouteMap> GetMaps()
        {
            return _routeMaps.Values;
        }

        public RouteMap GetMap(string areaName)
        {
            return _routeMaps.GetOrAdd(areaName ?? String.Empty, key => new RouteMap(key));
        }

        public void RemoveAlias(AliasInfoEx aliasInfo)
        {
            GetMap(aliasInfo.Area ?? String.Empty).Remove(aliasInfo);
        }
    }
}