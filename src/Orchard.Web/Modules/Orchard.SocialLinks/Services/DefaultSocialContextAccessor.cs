using Orchard.Mvc;
using Orchard.SocialLinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Core.Contents.PageContext;

namespace Orchard.SocialLinks.Services
{
    public class DefaultSocialContextAccessor : ISocialContextAccessor
    {
        private IHttpContextAccessor _httpContextAccessor;

        public DefaultSocialContextAccessor(IHttpContextAccessor httpContextAccessor, IOrchardServices orchardServices)
        {
            _httpContextAccessor = httpContextAccessor;
            Services = orchardServices;
        }
        public IOrchardServices Services { get; private set; }

        public SocialContext Current()
        {
            var context = new SocialContext();

            var httpContext = _httpContextAccessor.Current();
            if (httpContext != null)
            {
                context.Url = httpContext.Request.RawUrl;
            }

            if (Services.WorkContext != null)
            {
                var pcontext = Services.WorkContext.GetPageContext();
                if (pcontext != null && pcontext.ContentItem != null)
                    context.Content = pcontext.ContentItem;
            }

            if (context.Content != null || context.Url != null)
                return context;

            return null;
        }
    }
}