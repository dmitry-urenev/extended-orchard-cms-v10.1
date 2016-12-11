using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Orchard.StaticPages.Routing
{
    public class RouteInfo
    {
        /// <summary>
        /// StaticPage id
        /// </summary>
        public int Id { get; set; }

        public Route Route { get; set; }
    }
}
