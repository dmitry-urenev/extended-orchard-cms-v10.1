using Orchard.ContentManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.SocialLinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.SocialLinks.Services
{
    public interface ISocialShareService : IDependency
    {
        string Name { get; }

        Shape Apply(SocialContext context);
    }
}