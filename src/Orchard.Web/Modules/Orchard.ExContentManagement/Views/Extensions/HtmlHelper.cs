using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Orchard.ContentManagement;
using Orchard.Utility.Extensions;
using Orchard.Mvc.Html;
using System.Web.Routing;
using System.Dynamic;
using System.Collections.Generic;

namespace Orchard.ExContentManagement.Views
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ItemCreateLinkWithReturnUrl(this HtmlHelper html, string linkText, IContent content, object additionalRouteValues, object htmlAttributes)
        {
            var routeValues = new RouteValueDictionary();
            routeValues = routeValues.Merge(additionalRouteValues);
            routeValues["ReturnUrl"] = html.ViewContext.HttpContext.Request.RawUrl;

            return html.ItemCreateLink(linkText, content, routeValues, htmlAttributes);
        }
        public static MvcHtmlString ItemCreateLinkWithReturnUrl(this HtmlHelper html, string linkText, IContent content, object additionalRouteValues)
        {
            return html.ItemCreateLinkWithReturnUrl(linkText, content, additionalRouteValues, null);
        }

        public static MvcHtmlString ItemCreateLinkWithReturnUrl(this HtmlHelper html, string linkText, IContent content)
        {
            return html.ItemCreateLink(linkText, content, new { ReturnUrl = html.ViewContext.HttpContext.Request.RawUrl });
        }

        public static MvcHtmlString ItemCreateLink(this HtmlHelper html, string linkText, IContent content)
        {
            return html.ItemCreateLink(linkText, content, null);
        }

        public static MvcHtmlString ItemCreateLink(this HtmlHelper html, string linkText, IContent content, object additionalRouteValues, object htmlAttributes)
        {
            RouteValueDictionary routeValues = new RouteValueDictionary();
            routeValues = routeValues.Merge(additionalRouteValues);
            return html.ItemCreateLink(linkText, content, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ItemCreateLink(this HtmlHelper html, string linkText, IContent content, object additionalRouteValues)
        {
            return html.ItemCreateLink(linkText, content, additionalRouteValues, null);
        }

        public static MvcHtmlString ItemCreateLink(this HtmlHelper html, string linkText, IContent content, RouteValueDictionary additionalRouteValues)
        {
            return html.ItemCreateLink(linkText, content, additionalRouteValues, null);
        }

        public static MvcHtmlString ItemCreateLink(this HtmlHelper html, string linkText, IContent content, RouteValueDictionary additionalRouteValues, object htmlAttributes)
        {
            var metadata = content.ContentItem.ContentManager.GetItemMetadata(content);
            if (metadata.CreateRouteValues == null)
                return null;

            var attributes = htmlAttributes != null ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) : null;

            return html.ActionLink(
                linkText: NonNullOrEmpty(linkText, metadata.DisplayText, content.ContentItem.TypeDefinition.DisplayName),
                actionName: Convert.ToString(metadata.CreateRouteValues["action"]),
                routeValues: metadata.CreateRouteValues.Merge(additionalRouteValues),
                htmlAttributes: attributes);
        }


        private static string NonNullOrEmpty(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return null;
        }
    }
}