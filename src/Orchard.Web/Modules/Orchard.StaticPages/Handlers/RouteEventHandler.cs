using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.StaticPages.Models;
using Orchard.StaticPages.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.StaticPages.Handlers
{
    public class RouteEventHandler : IRouteEvents
    {
        private readonly IStaticPageService _staticPageService;

        public RouteEventHandler(IStaticPageService staticPageService)
        {
            _staticPageService = staticPageService;
        }

        public void Routed(IContent content, string path)
        {
            if (content.Has<StaticPagePart>())
            {
                var part = content.As<StaticPagePart>();
                _staticPageService.Route(part, path);
            }
        }
    }
}