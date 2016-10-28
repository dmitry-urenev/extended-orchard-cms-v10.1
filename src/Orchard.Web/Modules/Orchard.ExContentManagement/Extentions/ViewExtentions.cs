using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization.Services;

namespace Orchard.ExContentManagement
{
    public static class ViewExtentions
    {
        public static string LocalizedUrl<T>(this Orchard.Mvc.ViewPage<T> view, string relativeUrl)
        {
            return view.WorkContext.LocalizedUrl(relativeUrl);
        }
        public static string LocalizedUrl(this Orchard.Mvc.ViewPage view, string relativeUrl)
        {
            return view.WorkContext.LocalizedUrl(relativeUrl);
        }
        public static string LocalizedUrl(this WorkContext workContext, string relativeUrl)
        {
            var service = workContext.Resolve<ILocalizationService>();
            var cultureManager = workContext.Resolve<ICultureManager>();
            var currentCulture = cultureManager.GetCurrentCulture(workContext.HttpContext);

            var url = service.GetLocalizedPath(relativeUrl, currentCulture);
            return url;
        }
    }
}