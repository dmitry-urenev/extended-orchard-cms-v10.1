using System.Collections.Generic;
using Orchard.Alias.Implementation.Map;
using System.Web.Routing;
using Orchard.Alias.Implementation.Holder;

namespace Orchard.StaticPages.Routing
{
    /// <summary>
    /// Holds every alias in a tree structure, indexed by area
    /// </summary>
    public interface IRouteHolder : ISingletonDependency
    {
        /// <summary>
        /// Returns an <see cref="RouteMap"/> for a specific area
        /// </summary>
        RouteMap GetMap(string areaName);

        /// <summary>
        /// Returns all <see cref="RouteMap"/> instances
        /// </summary>
        IEnumerable<RouteMap> GetMaps();

        /// <summary>
        /// Adds or updates an alias in the tree
        /// </summary>
        void SetAlias(AliasInfoEx alias);

        /// <summary>
        /// Adds or updates a set of aliases in the tree
        /// </summary>
        void SetAliases(IEnumerable<AliasInfoEx> aliases);

        /// <summary>
        /// Removes an alias from the tree based on its path
        /// </summary>
        void RemoveAlias(AliasInfoEx aliasInfo);
    }
}