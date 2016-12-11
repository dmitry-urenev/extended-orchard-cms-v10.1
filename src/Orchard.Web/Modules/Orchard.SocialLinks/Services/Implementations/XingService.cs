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
    public class XingService : BaseSocialShareService, ISocialShareService
    {
        public XingService(IOrchardServices orchardServices)
            : base(orchardServices)
        {
        }

        public string Name
        {
            get { return "xing"; }
        }

        public override string BaseServiceUrl
        {
            get { return "https://www.xing.com/spi/shares/new"; }
        }

        public Shape Apply(SocialContext context)
        {
            var map = new Dictionary<string, string> { 
                { "url", "url" }
            };
            var parameters = base.GetParameters(context, map);
            parameters.Add("utm_source", "xing.com");
            parameters.Add("utm_medium", "social");
            parameters.Add("utm_campaign", "social-buttons");
            
            var shareUrl = base.ToUrl(parameters);

            var shape = Services.New.Parts_Share_Xing(ServiceName: Name, Url: shareUrl, Parameters: parameters);
            return shape;
        }
    }
}