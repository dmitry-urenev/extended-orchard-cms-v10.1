using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.StaticPages.ViewModels
{
    public class StaticPagePartEditViewModel
    {
        public StaticPagePartEditViewModel()
        {
            RouteValues = new List<RouteValue>();
        }

        public bool IsAction { get; set; }

        public string StaticPath { get; set; }
        
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public List<RouteValue> RouteValues { get; set; }
    }

    public class RouteValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
