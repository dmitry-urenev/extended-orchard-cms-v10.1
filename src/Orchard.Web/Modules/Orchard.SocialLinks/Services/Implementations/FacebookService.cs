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
    public class FacebookService : BaseSocialShareService, ISocialShareService
    {
        public FacebookService(IOrchardServices orchardServices)
            : base(orchardServices)
        {
        }

        public string Name
        {
            get { return "facebook"; }
        }

        public override string BaseServiceUrl
        {
            get { return "http://www.facebook.com/sharer.php"; }
        }

        public Shape Apply(SocialContext context)
        {
            var map = new Dictionary<string, string> { 
                { "url", "u" },
                { "title", "p[title]" }
            };
            var parameters = base.GetParameters(context, map);
            parameters.Add("p[url]", parameters["u"]);
            
            string url = parameters["u"];
            url += (url.Contains("?") ? "&" : "?");
            url += "utm_source=facebook.com&utm_medium=social&utm_campaign=social-buttons";

            parameters["u"] = url;

            var shareUrl = base.ToUrl(parameters);

            var shape = Services.New.Parts_Share_Facebook(ServiceName: Name, Url: shareUrl, Parameters: parameters);
            return shape;
        }
    }
}