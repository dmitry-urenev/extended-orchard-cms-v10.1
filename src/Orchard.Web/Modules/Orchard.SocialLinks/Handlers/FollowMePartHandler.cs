using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.SocialLinks.Models;
using Orchard.Data;

namespace Orchard.SocialLinks.Handlers
{
    public class FollowMePartHandler : ContentHandler
    {
        public FollowMePartHandler(IRepository<FollowMePartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}