using System;
using System.Web;
using System.Web.Mvc;
using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using Orchard.Environment.Configuration;
using Orchard.Core.Contents.PageContext;
using Orchard.ContentManagement.Aspects;

namespace Orchard.Localization.Selectors
{
    public class ContentCultureSelector : ICultureSelector
    {
        public const int SelectorPriority = -3;

        private readonly IWorkContextAccessor _workContextAccessor;

        public ContentCultureSelector(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }


        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            return EvaluateResult(context);
        }

        private CultureSelectorResult EvaluateResult(HttpContextBase context)
        {
            var wctx = _workContextAccessor.GetContext();
            var pageContext = wctx.GetPageContext();

            if (pageContext == null || pageContext.ContentItem == null)
                return null;

            var localized = pageContext.ContentItem.As<ILocalizableAspect>();
            if (localized == null || string.IsNullOrEmpty(localized.Culture))
            {
                return null;
            }

            return new CultureSelectorResult { Priority = -1, CultureName = localized.Culture };
        }
    }
}