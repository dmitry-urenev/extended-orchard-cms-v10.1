using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.SEO.Models
{
    public class RobotsMetaValues
    {
        public const string Default = "INDEX, FOLLOW";

        public const string NoIndexFollow = "NOINDEX, FOLLOW";

        public const string IndexNoFollow = "INDEX, NOFOLLOW";

        public const string NoIndexNoFollow = "NOINDEX, NOFOLLOW";
    }
}