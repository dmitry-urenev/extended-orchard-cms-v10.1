using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Orchard.StaticPages.Records;

namespace Orchard.StaticPages.Routing
{
    public interface IRouteStorage : IDependency
    {
        RouteRecord Set(string path, IDictionary<string, string> routeValues);
        RouteRecord Set(string path, string targetPath);

        IDictionary<string, string> Get(string path);
        RouteRecord Get(int id);

        void Remove(RouteRecord record);
        void Remove(string path);
        void Remove(int id);

        /// <summary>
        /// Tuple: Path, Area, RouteValues, Id
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<Tuple<string, string, IDictionary<string, string>, int>> List(Expression<Func<RouteRecord, bool>> predicate);
        IEnumerable<Tuple<string, string, IDictionary<string, string>, int>> List();
    }
}