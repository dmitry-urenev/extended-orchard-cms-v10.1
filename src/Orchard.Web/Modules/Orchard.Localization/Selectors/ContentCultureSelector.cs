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

        private readonly IPageContextHolder _pageContextHolder;

        public ContentCultureSelector(IPageContextHolder pageContextHolder)
        {
            _pageContextHolder = pageContextHolder;
        }


        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            return EvaluateResult(context);
        }

        private CultureSelectorResult EvaluateResult(HttpContextBase context)
        {
            var pageContext = _pageContextHolder.PageContext;
            if (pageContext == null && pageContext.ContentItem == null)
                return null;

            var localized = pageContext.ContentItem.As<ILocalizableAspect>();
            if (localized == null || string.IsNullOrEmpty(localized.Culture))
            {
                return null;
            }

            return new CultureSelectorResult { Priority = SelectorPriority, CultureName = localized.Culture };
        }
    }
}