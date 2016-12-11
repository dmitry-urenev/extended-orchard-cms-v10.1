using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.StaticPages.ViewModels
{
    public class StaticRouteViewModel
    {
        public int ID { get; set; }

        public string Path { get; set; }

        public string RouteValues { get; set; }
    }
}