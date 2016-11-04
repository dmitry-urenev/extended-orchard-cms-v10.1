using Orchard.SEO.Models;
using Orchard.Mvc.Filters;
using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Environment.Extensions;

namespace Orchard.SEO.Filters
{
    [OrchardFeature("Orchard.SEO")]
    public class MetaResultFilter : FilterProvider, IResultFilter
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public MetaResultFilter(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // layers and widgets should only run on a full view rendering result
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null)
                return;

            var workContext = _workContextAccessor.GetContext(filterContext);

            if (workContext == null ||
                workContext.Layout == null ||
                workContext.CurrentSite == null)
            {
                return;
            }

            var resourceManager = workContext.Resolve<IResourceManager>();

            var robotsMeta = resourceManager.GetRegisteredMetas()
                .FirstOrDefault(m => m.Name.Equals("robots", StringComparison.OrdinalIgnoreCase));

            if (robotsMeta == null)
            {
                robotsMeta = new MetaEntry { Name = "robots", Content = RobotsMetaValues.Default };
                resourceManager.SetMeta(robotsMeta);
            }
            else if (string.IsNullOrEmpty(robotsMeta.Content))
            {
                robotsMeta.Content = RobotsMetaValues.Default;
            }

            var viewMeta = viewResult.ViewData.Where(d => d.Key.IndexOf("META:", StringComparison.OrdinalIgnoreCase) != -1)
                .Select(d => new { Key = d.Key.Replace("META:", "").ToLower(), Value = (string)d.Value })
                .ToList();

            foreach (var m in viewMeta)
            {
                if ("title".Equals(m.Key, StringComparison.OrdinalIgnoreCase))
                {
                    workContext.Layout.HeaderTitle = m.Value;
                }
                resourceManager.SetMeta(new MetaEntry { Name = m.Key, Content = m.Value });
            }
        }


        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}