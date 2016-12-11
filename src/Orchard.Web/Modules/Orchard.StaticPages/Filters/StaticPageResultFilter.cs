using Orchard.ContentManagement;
using Orchard.Core.Contents.PageContext;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;
using Orchard.Themes;
using Orchard.UI;
using Orchard.UI.Admin;
using Orchard.UI.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orchard.StaticPages.Filters
{
    [OrchardFeature("Orchard.StaticPages.ContentParts")]
    public class StaticPageResultFilter : FilterProvider, IResultFilter
    {
        private readonly IPageContextHolder _pageContextHolder;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public StaticPageResultFilter(IWorkContextAccessor workContextAccessor, 
            IPageContextHolder pageContextHolder, IContentManager contentManager)
        {
            _workContextAccessor = workContextAccessor;
            _pageContextHolder = pageContextHolder;
            _contentManager = contentManager;
        }


        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var dataTokens = filterContext.RouteData.DataTokens;
            if (!dataTokens.ContainsKey("_staticUrl") || 
                _pageContextHolder.PageContext.ContentItem == null)
                return;

            // layers and widgets should only run on a full view rendering result
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null)
                return;

            var workContext = _workContextAccessor.GetContext(filterContext);
            var contentItem = _pageContextHolder.PageContext.ContentItem;

            if (workContext == null ||
                workContext.Layout == null ||
                workContext.CurrentSite == null)
            {
                return;
            }

            var shape = _contentManager.BuildDisplay(contentItem);
            var zones = workContext.Layout.Zones;

            //string[] zonesToMerge = new[] { "Meta", "Header", "Content" };
            //foreach (var p in contentItem.Parts)
            //{
            //    var partShape = _contentManager.BuildDisplay(p);
            //    zones["Content"].Add(partShape);
            //}

            zones["Content"].Add(shape, "0.1");
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}