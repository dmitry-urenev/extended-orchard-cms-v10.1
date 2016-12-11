using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.StaticPages.Models;
using Orchard.StaticPages.Routing;
using Orchard.StaticPages.ViewModels;
using Orchard.Themes;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Orchard.StaticPages.Controllers
{
    [Themed, Admin]
    public class PageController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IRouteStorage _routeStorage;

        public PageController(IContentManager contentManager, IRouteStorage routeStorage)
        {
            _contentManager = contentManager;
            _routeStorage = routeStorage;
        }
        
        public ActionResult Transfer(string url)
        {
            return new TransferResult(url);
        }

        public ActionResult Display(int id)
        {
            var page = _contentManager.Get<StaticPagePart>(id);
            if (page == null)
                return HttpNotFound();

            var url = page.StaticPageUrl;

            if (page.Has<IAliasAspect>())
            {
                if (page.ContentItem.IsPublished())
                    url = page.As<IAliasAspect>().Path;
            }

            return new RedirectResult("~/" + url);
        }

        public ActionResult List()
        {
            var routes = _routeStorage.List()
                .Select(r => new StaticRouteViewModel()
                {
                    ID = r.Item4,
                    Path = r.Item1,
                    RouteValues = string.Join(", ", r.Item3.Select(kv => string.Format("{0}: \"{1}\"", kv.Key, kv.Value)).ToArray())
                })
                .ToList();
            return View(routes);
        }

	}
}