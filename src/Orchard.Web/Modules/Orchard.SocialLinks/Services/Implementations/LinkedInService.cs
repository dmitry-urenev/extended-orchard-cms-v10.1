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
    public class LinkedInService : BaseSocialShareService, ISocialShareService
    {
        public LinkedInService(IOrchardServices orchardServices)
            : base(orchardServices)
        {
        }

        public string Name
        {
            get { return "linkedin"; }
        }

        public override string BaseServiceUrl
        {
            get { return "https://www.linkedin.com/shareArticle"; }
        }

        public Shape Apply(SocialContext context)
        {
            var map = new Dictionary<string, string> { 
                { "url", "url" },
                { "title", "title" },
                { "source", "source" }
            };
            var parameters = base.GetParameters(context, map);
            parameters.Add("mini", "true");

            var shareUrl = base.ToUrl(parameters);

            var shape = Services.New.Parts_Share_LinkedIn(ServiceName: Name, Url: shareUrl, Parameters: parameters);
            return shape;
        }
    }
}