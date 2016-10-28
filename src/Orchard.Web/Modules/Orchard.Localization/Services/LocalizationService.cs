using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization.Models;
using Orchard.Autoroute.Services;
using Orchard.ContentManagement.Aspects;
using Orchard.Autoroute.Models;

namespace Orchard.Localization.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IContentManager _contentManager;
        private readonly ICultureManager _cultureManager;
        private readonly IPathResolutionService _pathResolutionService;

        public LocalizationService(
            IContentManager contentManager,
            ICultureManager cultureManager,
            IPathResolutionService pathResolutionService)
        {
            _contentManager = contentManager;
            _cultureManager = cultureManager;
            _pathResolutionService = pathResolutionService;
        }

        public LocalizationPart GetLocalizedContentItem(IContent content, string culture)
        {
            // Warning: Returns only the first of same culture localizations.
            return ((ILocalizationService)this).GetLocalizedContentItem(content, culture, null);
        }

        public LocalizationPart GetLocalizedContentItem(IContent content, string culture, VersionOptions versionOptions)
        {
            var cultureRecord = _cultureManager.GetCultureByName(culture);

            if (cultureRecord == null)
                return null;

            var localized = content.As<LocalizationPart>();

            if (localized == null)
                return null;

            if (culture.Equals(localized.Culture?.Culture))
            {
                return localized;
            }

            // For Example: US - Master content
                return _contentManager
                    .Query<LocalizationPart>(versionOptions, content.ContentItem.ContentType)
                    .Where<LocalizationPartRecord>(l =>
                        (l.MasterContentItemId == localized.Record.MasterContentItemId || // AF -> DE: the same MasterContentItemId
                         l.MasterContentItemId == content.ContentItem.Id ||  // US -> DE: current content is master
                         l.Id == localized.Record.MasterContentItemId // AF -> US
                         ) 
                        && l.CultureId == cultureRecord.Id)
                    .Slice(1)
                    .FirstOrDefault();
        }

        public string GetContentCulture(IContent content)
        {
            var localized = content.As<LocalizationPart>();
            return localized != null && localized.Culture != null
                ? localized.Culture.Culture
                : _cultureManager.GetSiteCulture();
        }

        public void SetContentCulture(IContent content, string culture)
        {
            var localized = content.As<LocalizationPart>();
            if (localized == null)
                return;

            localized.Culture = _cultureManager.GetCultureByName(culture);
        }

        public IEnumerable<LocalizationPart> GetLocalizations(IContent content)
        {
            // Warning: May contain more than one localization of the same culture.
            return ((ILocalizationService)this).GetLocalizations(content, null);
        }

        public IEnumerable<LocalizationPart> GetLocalizations(IContent content, VersionOptions versionOptions)
        {
            if (content.ContentItem.Id == 0 || !content.Has<LocalizationPart>()) // new item or item that was created before adding LocalizationPart
                return Enumerable.Empty<LocalizationPart>();

            var localized = content.As<LocalizationPart>();

            var query = versionOptions == null
                ? _contentManager.Query<LocalizationPart>(localized.ContentItem.ContentType)
                : _contentManager.Query<LocalizationPart>(versionOptions, localized.ContentItem.ContentType);

            int contentItemId = localized.ContentItem.Id;

            if (localized.HasTranslationGroup)
            {
                int masterContentItemId = localized.MasterContentItem.ContentItem.Id;

                query = query.Where<LocalizationPartRecord>(l =>
                    l.Id != contentItemId // Exclude the content
                    && (l.Id == masterContentItemId || l.MasterContentItemId == masterContentItemId));
            }
            else {
                query = query.Where<LocalizationPartRecord>(l =>
                    l.MasterContentItemId == contentItemId);
            }

            // Warning: May contain more than one localization of the same culture.
            return query.List().ToList();
        }

        public string GetLocalizedPath(string url, string culture)
        {
            string localizedPath = string.Empty;
            var routeInfo = _pathResolutionService.ResolveUrl(url);

            if (routeInfo == null || !routeInfo.Content.Parts.Any(p => p.Is<ILocalizableAspect>()))
                return localizedPath;

            var localizationPart = GetLocalizedContentItem(routeInfo.Content, culture, VersionOptions.Published);
            if (localizationPart != null && localizationPart.ContentItem.Has<AutoroutePart>())
            {
                var contentItem = localizationPart.ContentItem;

                AutoroutePart autoroutePart = contentItem.As<AutoroutePart>();
                if (routeInfo.RouteValues != null)
                {
                    localizedPath = RouteUtils.GetVirtualPath(autoroutePart.Path, routeInfo.RouteValues);
                }
                else
                {
                    localizedPath = autoroutePart.Path;
                }
            }

            return localizedPath;
        }

        public string GetLocalizedPath(IContent content, string culture)
        {
            var localizationPart = GetLocalizedContentItem(content, culture, VersionOptions.Published);
            if (localizationPart != null && localizationPart.ContentItem.Has<AutoroutePart>())
            {
                var contentItem = localizationPart.ContentItem;
                AutoroutePart autoroutePart = localizationPart.ContentItem.As<AutoroutePart>();
                return autoroutePart.Path;
            }

            return string.Empty;
        }
    }
}