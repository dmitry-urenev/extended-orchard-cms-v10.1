using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Utilities;
using Orchard.StaticPages.Records;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orchard.StaticPages.Models
{
    public class StaticPagePart : ContentPart<StaticPagePartRecord>
    {
        private readonly LazyField<RouteRecord> _route = new LazyField<RouteRecord>();

        public LazyField<RouteRecord> RouteField { get { return _route; } }

        public RouteRecord Route
        {
            get { return _route.Value; }
            set { _route.Value = value; }
        }

        public bool IsAction
        {
            get { return Retrieve(x => x.IsAction); }
            set { Store(x => x.IsAction, value); }
        }
             
        public string StaticPageUrl
        {
            get { return Retrieve(x => x.StaticPageUrl); }
            set { Store(x => x.StaticPageUrl, value); }
        }

        /// <summary>
        /// Temprory storage untill OnPublished event
        /// </summary>
        public IDictionary<string, string> RouteValues { get; set; }
    }
}
