using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.UI.Notify;
using Orchard.StaticPages.Models;
using System.Web.Routing;
using Orchard.ContentManagement.Aspects;
using Orchard.Alias;
using Orchard.StaticPages.Records;
using Orchard.StaticPages.Routing;
using Orchard.StaticPages.Services;

namespace Orchard.StaticPages.Handlers {
    public class StaticPageHandler : ContentHandler {

        private readonly IStaticPageService _staticPageService;
        private readonly IRouteStorage _routeStorage;

        private readonly string AliasSource = "StaticPages:View";

        public StaticPageHandler(
            IRepository<StaticPagePartRecord> staticPageRepository,
            IRouteStorage routeStorage,
            IStaticPageService staticPageService)
        {
            _staticPageService = staticPageService;
            _routeStorage = routeStorage;

            Filters.Add(StorageFilter.For(staticPageRepository));

            OnActivated<StaticPagePart>(PropertySetHandlers);
            OnLoading<StaticPagePart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<StaticPagePart>((context, part, versionedPart) => LazyLoadHandlers(versionedPart));

            // see RouteEventHandler 

            OnRemoved<StaticPagePart>((ctx, part) => RemoveAlias(part));
            OnUnpublished<StaticPagePart>((ctx, part) => RemoveAlias(part));

            // Register alias as identity
            OnGetContentItemMetadata<StaticPagePart>((ctx, part) =>
            {
                if (part.Has<IAliasAspect>() && part.As<IAliasAspect>().Path != null)
                    ctx.Metadata.Identity.Add("alias", part.As<IAliasAspect>().Path);
            });
        }

        private void RemoveAlias(StaticPagePart part)
        {
            _staticPageService.RemoveAliases(part);
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var page = context.ContentItem.As<StaticPagePart>();
            if (page == null)
                return;

            var path = page.As<IAliasAspect>().Path;            

            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", "Orchard.StaticPages"},
                {"Controller", "Page"},
                {"Action", "Display"},
                {"path", path }
            };
        }

        protected static void PropertySetHandlers(ActivatedContentContext context, StaticPagePart staticPagePart)
        {
            staticPagePart.RouteField.Setter(routeRecord =>
            {
                staticPagePart.Record.RouteId = routeRecord.Id;
                return routeRecord;
            });
        }

        protected void LazyLoadHandlers(StaticPagePart staticPagePart)
        {
            staticPagePart.RouteField.Loader(ctx =>
                _routeStorage.Get(staticPagePart.Record.RouteId));
        }
    }
}
