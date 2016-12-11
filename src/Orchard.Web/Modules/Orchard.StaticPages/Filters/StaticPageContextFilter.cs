using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Core.Contents.PageContext;
using Orchard.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orchard.StaticPages.Filters
{
    public class StaticPageContextFilter : FilterProvider, IActionFilter
    {
        private readonly IPageContextHolder _pageContextHolder;
        private readonly IContentManager _contentManager;

        public StaticPageContextFilter(IPageContextHolder pageContextHolder, IContentManager contentManager)
        {
            _pageContextHolder = pageContextHolder;
            _contentManager = contentManager;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dataTokens = filterContext.RouteData.DataTokens;
            if (dataTokens.ContainsKey("_staticUrl"))
            {
                var url = (string)dataTokens["_staticUrl"];
                var staticPage = _contentManager
                    .Query<AutoroutePart, AutoroutePartRecord>()
                    .Where(r => r.DisplayAlias == url)
                    .List()
                    .FirstOrDefault();
                
                if (staticPage != null)
                {
                    _pageContextHolder.PageContext.ContentItem = staticPage.ContentItem;
                }
            }
        }
    }
}