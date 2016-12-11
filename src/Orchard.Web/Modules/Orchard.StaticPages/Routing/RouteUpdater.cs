using System;
using System.Linq;
using Orchard.Alias.Implementation.Holder;
using Orchard.Alias.Implementation.Storage;
using Orchard.Environment;
using Orchard.Tasks;
using Orchard.Logging;

namespace Orchard.StaticPages.Routing
{
    public class RoutesHolderUpdater : IOrchardShellEvents, IBackgroundTask
    {
        private readonly IRouteHolder _routeHolder;
        private readonly IRouteStorage _storage;
        private readonly IRouteUpdateCursor _cursor;

        public ILogger Logger { get; set; }

        public RoutesHolderUpdater(IRouteHolder routeHolder, IRouteStorage storage, IRouteUpdateCursor cursor)
        {
            _routeHolder = routeHolder;
            _storage = storage;
            _cursor = cursor;
            Logger = NullLogger.Instance;
        }

        void IOrchardShellEvents.Activated()
        {
            Refresh();
        }

        void IOrchardShellEvents.Terminating()
        {
        }

        private void Refresh()
        {
            try
            {
                // only retreive aliases which have not been processed yet
                var routesInfo = _storage.List(x => x.Id > _cursor.Cursor).ToArray();

                // update the last processed id
                if (routesInfo.Any())
                {
                    _cursor.Cursor = routesInfo.Last().Item4; // Id
                    _routeHolder.SetAliases(routesInfo.Select(alias => new AliasInfoEx { Id = alias.Item4, Path = alias.Item1, Area = alias.Item2, RouteValues = alias.Item3 }));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception during Routes refresh");
            }
        }

        public void Sweep()
        {
            Refresh();
        }
    }
}
