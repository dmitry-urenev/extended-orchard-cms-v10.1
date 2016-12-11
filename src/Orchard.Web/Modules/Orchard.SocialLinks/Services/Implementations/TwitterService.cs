using Orchard.DisplayManagement;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.SocialLinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Core.Title.Models;
using Orchard.ContentManagement.Aspects;
using System.Collections.Specialized;

namespace Orchard.SocialLinks.Services.Implementations
{
    public class TwitterService : BaseSocialShareService, ISocialShareService
    {
        public TwitterService(IOrchardServices orchardServices)
            : base(orchardServices)
        {
        }

        public string Name
        {
            get { return "twitter"; }
        }

        public override string BaseServiceUrl
        {
            get { return "https://twitter.com/intent/tweet"; }
        }

        public Shape Apply(SocialContext context)
        {
            var map = new Dictionary<string, string> { 
                { "url", "url" },
                { "title", "text" },
                { "hashtags", "hashtags" }
            };
            var parameters = base.GetParameters(context, map);

            var shareUrl = base.ToUrl(parameters);

            var shape = Services.New.Parts_Share_Twitter(ServiceName: Name, Url: shareUrl, Parameters: parameters);
            return shape;
        }
    }
}