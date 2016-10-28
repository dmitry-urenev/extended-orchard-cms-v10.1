using System.Linq;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using System.Web.Routing;

namespace Orchard.Autoroute.Services
{
    public class PathResolutionService : IPathResolutionService
    {
        private readonly IContentManager _contentManager;
        private readonly IRepository<AutoroutePartRecord> _autorouteRepository;
        private readonly IHomeAliasService _homeAliasService;

        public PathResolutionService(
            IRepository<AutoroutePartRecord> autorouteRepository,
            IHomeAliasService homeAliasService,
            IContentManager contentManager)
        {
            _contentManager = contentManager;
            _autorouteRepository = autorouteRepository;
            _homeAliasService = homeAliasService;
        }

        public AutoroutePart GetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                var homePage = _homeAliasService.GetHomePage();
                return homePage != null ? homePage.As<AutoroutePart>() : null;
            }

            var autorouteRecord = _autorouteRepository.Table
                .FirstOrDefault(part => part.DisplayAlias == path && part.ContentItemVersionRecord.Published);

            if (autorouteRecord == null)
            {
                return null;
            }

            return _contentManager.Get(autorouteRecord.ContentItemRecord.Id).As<AutoroutePart>();
        }


        public AutorouteInfo ResolveUrl(string url)
        {
            string alias = url.TrimStart('~', '/');
            string virtualPath = "~/" + alias;

            AutoroutePart routePart;
            RouteValueDictionary routeValues = null;

            var routeData = RouteUtils.GetRouteDataByUrl(virtualPath);
            if (routeData == null || !(routeData.Route is Route))
            {
                routePart = GetPath(alias);
            }
            else
            {
                var route = routeData.Route as Route;
                alias = route.Url.TrimStart('~', '/');

                routePart = GetPath(alias);
                routeValues = routeData.Values;
            }

            if (routePart != null)
                return new AutorouteInfo
                {
                    Content = routePart.ContentItem,
                    RouteValues = routeValues
                };

            return null;
        }
    }
}
