using Orchard.StaticPages.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Orchard.StaticPages.Routing
{
    public static class RouteUtils
    {
        public static IDictionary<string, string> ParseRouteValues(string routeValuesString)
        {
            IDictionary<string, string> routeValues = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(routeValuesString))
            {
                foreach (var attr in XElement.Parse(routeValuesString).Attributes())
                {
                    routeValues.Add(attr.Name.LocalName, attr.Value);
                }
            }
            return routeValues;
        }

        public static string SerializeRouteValues(IDictionary<string, string> routeValues)
        {
            var values = new XElement("Route");
            foreach (var routeValue in routeValues.OrderBy(kv => kv.Key, StringComparer.InvariantCultureIgnoreCase))
            {
                values.SetAttributeValue(routeValue.Key, routeValue.Value ?? "");
            }
            return values.ToString();
        }

    }
}