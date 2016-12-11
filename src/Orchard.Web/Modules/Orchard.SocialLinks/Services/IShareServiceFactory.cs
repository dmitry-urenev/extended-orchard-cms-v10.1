using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.SocialLinks.Services
{
    public interface IShareServiceFactory : IDependency
    {
        ISocialShareService GetService(string name);
    }
}