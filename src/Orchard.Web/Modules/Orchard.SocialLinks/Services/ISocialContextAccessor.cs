using Orchard.SocialLinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.SocialLinks.Services
{
    public interface ISocialContextAccessor : IDependency
    {
        SocialContext Current();    
    }
}