using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.SocialLinks.Models
{
    public class SocialContext
    {
        public IContent Content { get; set; }

        public string Url { get; set; }
    }
}