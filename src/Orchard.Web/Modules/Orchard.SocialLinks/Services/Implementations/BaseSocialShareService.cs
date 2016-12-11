using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.SocialLinks.Models;
using Orchard.Tags.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Orchard.SocialLinks.Services.Implementations
{
    public abstract class BaseSocialShareService
    {
        public BaseSocialShareService(IOrchardServices orchardServices)
        {
            Services = orchardServices;
        }

        public IOrchardServices Services { get; private set; }


        public abstract string BaseServiceUrl { get; }

        public string ToUrl(NameValueCollection parameters)
        {
            var array = (from key in parameters.AllKeys
                         from value in parameters.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return BaseServiceUrl + "?" + string.Join("&", array);
        }

        protected NameValueCollection GetParameters(SocialContext context)
        {
            var parameters = new NameValueCollection();

            string baseUrl = string.IsNullOrEmpty(Services.WorkContext.CurrentSite.BaseUrl) ? "" :
                Services.WorkContext.CurrentSite.BaseUrl.TrimEnd('/');

            if (!string.IsNullOrEmpty(baseUrl))
            {
                string source = baseUrl.Replace("http://", "").Replace("https://", "");
                parameters.Add("source", source);
            }

            parameters.Add("url", context.Url);

            if (context.Content != null && context.Content.ContentItem != null)
            {
                var contentItem = context.Content.ContentItem;

                if (contentItem.Has<TitlePart>())
                {
                    parameters.Add("title", contentItem.As<TitlePart>().Title);
                }
                if (contentItem.Has<AutoroutePart>())
                {
                    parameters["url"] = baseUrl + "/" + contentItem.As<AutoroutePart>().Path;
                }
                if (contentItem.Has<TagsPart>())
                {
                    var tags = contentItem.As<TagsPart>().CurrentTags;
                    if (tags != null && tags.Any())
                        parameters["hashtags"] = string.Join(",", tags);
                }
            }

            return parameters;
        }

        protected NameValueCollection GetParameters(SocialContext context, Dictionary<string, string> map)
        {
            var ps = GetParameters(context);
            var transformed = new NameValueCollection();
            foreach (string key in ps.AllKeys)
            {
                if (map.ContainsKey(key))
                    transformed.Add(map[key], ps[key]);
            }
            return transformed;
        }
    }
}