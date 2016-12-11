using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Orchard.Data;
using Orchard.StaticPages.Records;
using Orchard.Alias.Records;
using Orchard.Alias.Implementation.Holder;

namespace Orchard.StaticPages.Routing 
{   
    public class RouteStorage  : IRouteStorage 
    {
        private readonly IRepository<RouteRecord> _routeRepository;
        private readonly IRepository<ActionRecord> _actionRepository;
        private readonly IRouteHolder _routeHolder;

        public RouteStorage(IRepository<RouteRecord> routeRepository, IRepository<ActionRecord> actionRepository,
            IRouteHolder routeHolder) {
            _routeRepository = routeRepository;
            _actionRepository = actionRepository;
            _routeHolder = routeHolder;
        }

        public RouteRecord Set(string path, string targetPath)
        {
            var values = new Dictionary<string, string> {
                { "area", "Orchard.StaticPages"},
                { "controller", "Page"},
                { "action", "Transfer"},
                { "url", targetPath }
            };

            return Set(path, values);
        }

        public RouteRecord Set(string path, IDictionary<string, string> routeValues)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            var routeRecord = _routeRepository.Fetch(r => r.Path == path, o => o.Asc(r => r.Id), 0, 1).FirstOrDefault();
            routeRecord = routeRecord ?? new RouteRecord { Path = path };

            string areaName = null;
            string controllerName = null;
            string actionName = null;

            var values = new XElement("Route");
            foreach (var routeValue in routeValues.OrderBy(kv => kv.Key, StringComparer.InvariantCultureIgnoreCase))
            {
                if (string.Equals(routeValue.Key, "area", StringComparison.InvariantCultureIgnoreCase))
                {
                    areaName = routeValue.Value;
                }
                else if (string.Equals(routeValue.Key, "controller", StringComparison.InvariantCultureIgnoreCase))
                {
                    controllerName = routeValue.Value;
                }
                else if (string.Equals(routeValue.Key, "action", StringComparison.InvariantCultureIgnoreCase))
                {
                    actionName = routeValue.Value;
                }
                else
                {
                    values.SetAttributeValue(routeValue.Key, routeValue.Value ?? "");
                }
            }

            routeRecord.Action = _actionRepository.Fetch(
                r => r.Area == areaName && r.Controller == controllerName && r.Action == actionName,
                o => o.Asc(r => r.Id), 0, 1).FirstOrDefault();

            routeRecord.Action = routeRecord.Action ?? new ActionRecord { Area = areaName, Controller = controllerName, Action = actionName };

            routeRecord.RouteValues = values.ToString();
            if (routeRecord.Action.Id == 0 || routeRecord.Id == 0)
            {
                if (routeRecord.Action.Id == 0)
                {
                    _actionRepository.Create(routeRecord.Action);
                }
                if (routeRecord.Id == 0)
                {
                    _routeRepository.Create(routeRecord);
                }
                // Bulk updates might go wrong if we don't flush
                _routeRepository.Flush();
            }
            // Transform and push into RouteHolder
            var dict = ToDictionary(routeRecord);
            _routeHolder.SetAlias(new AliasInfoEx { Id = dict.Item4, Path = dict.Item1, Area = dict.Item2, RouteValues = dict.Item3 });

            return routeRecord;
        }
        
        public IDictionary<string, string> Get(string path)
        {
            return _routeRepository
                .Fetch(r => r.Path == path, o => o.Asc(r => r.Id), 0, 1)
                .Select(ToDictionary)
                .Select(item => item.Item3)
                .SingleOrDefault();
        }

        public RouteRecord Get(int id)
        {
            return _routeRepository
                .Fetch(r => r.Id == id, o => o.Asc(r => r.Id), 0, 1)
                .SingleOrDefault();
        }

        public void Remove(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            foreach (var routeRecord in _routeRepository.Fetch(r => r.Path == path))
            {
                _routeRepository.Delete(routeRecord);
                // Bulk updates might go wrong if we don't flush
                _routeRepository.Flush();
                var dict = ToDictionary(routeRecord);
                _routeHolder.RemoveAlias(new AliasInfoEx() { Id = dict.Item4, Path = dict.Item1, Area = dict.Item2, RouteValues = dict.Item3 });
            }
        }

        public void Remove(RouteRecord routeRecord)
        {
            _routeRepository.Delete(routeRecord);
            _routeRepository.Flush();
            var dict = ToDictionary(routeRecord);
            _routeHolder.RemoveAlias(new AliasInfoEx() { Id = dict.Item4, Path = dict.Item1, Area = dict.Item2, RouteValues = dict.Item3 });
        }

        public void Remove(int id)
        {
            if (id == 0)
                return;

            var routeRecord = _routeRepository.Fetch(r => r.Id == id).SingleOrDefault();
            if (routeRecord != null)
            {
                _routeRepository.Delete(routeRecord);
                _routeRepository.Flush();
                var dict = ToDictionary(routeRecord);
                _routeHolder.RemoveAlias(new AliasInfoEx() { Id = dict.Item4, Path = dict.Item1, Area = dict.Item2, RouteValues = dict.Item3 });
            }
        }

        public IEnumerable<Tuple<string, string, IDictionary<string, string>, int>> List()
        {
            return List((Expression<Func<RouteRecord, bool>>)null);
        }

        public IEnumerable<Tuple<string, string, IDictionary<string, string>, int>> List(Expression<Func<RouteRecord, bool>> predicate)
        {
            var table = _routeRepository.Table;

            if (predicate != null)
            {
                table = table.Where(predicate);
            }

            return table.OrderBy(a => a.Id).Select(ToDictionary).ToList();
        }

        private static Tuple<string, string, IDictionary<string, string>, int> ToDictionary(RouteRecord routeRecord)
        {
            IDictionary<string, string> routeValues = new Dictionary<string, string>();
            if (routeRecord.Action != null)
            {
                if (routeRecord.Action.Area != null)
                {
                    routeValues.Add("area", routeRecord.Action.Area);
                }
                if (routeRecord.Action.Controller != null)
                {
                    routeValues.Add("controller", routeRecord.Action.Controller);
                }
                if (routeRecord.Action.Action != null)
                {
                    routeValues.Add("action", routeRecord.Action.Action);
                }
            }
            if (!string.IsNullOrEmpty(routeRecord.RouteValues))
            {
                foreach (var attr in XElement.Parse(routeRecord.RouteValues).Attributes())
                {
                    routeValues.Add(attr.Name.LocalName, attr.Value);
                }
            }
            return Tuple.Create(routeRecord.Path, routeRecord.Action.Area, routeValues, routeRecord.Id);
        }
    }
}