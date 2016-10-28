using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Autoroute.Settings;

namespace Orchard.ExContentManagement.Aliases
{
    [OrchardSuppressDependency("Orchard.Autoroute.Services.DefaultAliasPatternProvider")]
    public class AliasPatternProvider : IAliasPatternProvider
    {
        private readonly IAliasPatternProvider _defaultProvider;
        private readonly ICultureManager _cultureManager;
        private List<string> _monitoredContentTypes;

        public AliasPatternProvider(IContentDefinitionManager contentDefinitionManager, 
            ICultureManager cultureManager)
        {
            _defaultProvider = new DefaultAliasPatternProvider(contentDefinitionManager, cultureManager);
            _cultureManager = cultureManager;

            _monitoredContentTypes = new List<string>() {
                "Page", "ProjectionPage", "SpecialEventContent"
            };
        }

        public string GetPatternFor(IContent content)
        {
            if (!_monitoredContentTypes.Contains(content.ContentItem.ContentType, StringComparer.OrdinalIgnoreCase))
                return _defaultProvider.GetPatternFor(content);

            if (content.Has<ICommonPart>())
            {
                if (content.As<ICommonPart>().Container != null)
                {
                    return "{Content.ParentPath}{Content.Slug}";
                }
            }
            if (content.Has<ILocalizableAspect>())
            {
                string culture = content.As<ILocalizableAspect>().Culture;
                string siteCulture = _cultureManager.GetSiteCulture();
                if (culture != null && !string.Equals(culture, siteCulture, StringComparison.OrdinalIgnoreCase))
                {
                    return "{Content.Culture}/{Content.Slug}";
                }
                else
                {
                    return "{Content.Slug}";
                }
            }
            return _defaultProvider.GetPatternFor(content);
        }

        public Autoroute.Settings.RoutePattern GetDefaultPattern(string contentType, string culture)
        {
            return _defaultProvider.GetDefaultPattern(contentType, culture);
        }

        public void CreatePattern(string contentType, string name, string pattern, string description, bool makeDefault)
        {
            _defaultProvider.CreatePattern(contentType, name, pattern, description, makeDefault);
        }

        public IEnumerable<Autoroute.Settings.RoutePattern> GetPatterns(string contentType)
        {
            return _defaultProvider.GetPatterns(contentType);
        }
    }
}