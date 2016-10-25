using Orchard.Autoroute.Models;
using Orchard.Autoroute.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Autoroute.Services
{
    public class DefaultAliasPatternProvider : IAliasPatternProvider
    {
        private readonly ICultureManager _cultureManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public DefaultAliasPatternProvider(
            IContentDefinitionManager contentDefinitionManager,
            ICultureManager cultureManager)
        {
            _cultureManager = cultureManager;
            _contentDefinitionManager = contentDefinitionManager;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }


        public string GetPatternFor(IContent content)
        {
            // var settings = part.TypePartDefinition.Settings.GetModel<AutorouteSettings>();
            //if (settings.UseCulturePattern)
            //{
            //    // TODO: Refactor the below so that we don't need to know about Request.Form["Localization.SelectedCulture"].
            //    // If we are creating from a form post we use the form value for culture.
            //    var context = _httpContextAccessor.Current();
            //    var selectedCulture = context.Request.Form["Localization.SelectedCulture"];
            //    if (!String.IsNullOrEmpty(selectedCulture))
            //    {
            //        itemCulture = selectedCulture;
            //    }
            //}

            string itemCulture = _cultureManager.GetSiteCulture();
            // If we are editing an existing content item.
            if (content.Has<ILocalizableAspect>())
            {
                var aspect = content.As<ILocalizableAspect>();
                if (aspect != null)
                {
                    itemCulture = aspect.Culture;
                }
            }

            string pattern = GetDefaultPattern(content.ContentItem.ContentType, itemCulture).Pattern;
            if (content.Has<AutoroutePart>())
            {
                var part = content.As<AutoroutePart>();

                // String.Empty forces pattern based generation. "/" forces homepage
                if (part.UseCustomPattern
                    && (!String.IsNullOrWhiteSpace(part.CustomPattern) || String.Equals(part.CustomPattern, "/")))
                {
                    pattern = part.CustomPattern;
                }
            }

            return pattern;
        }

        public void CreatePattern(string contentType, string name, string pattern, string description, bool makeDefault)
        {
            var contentDefinition = _contentDefinitionManager.GetTypeDefinition(contentType);

            if (contentDefinition == null)
            {
                throw new OrchardException(T("Unknown content type: {0}", contentType));
            }

            var settings = contentDefinition.Settings.GetModel<AutorouteSettings>();

            var routePattern = new RoutePattern
            {
                Description = description,
                Name = name,
                Pattern = pattern,
                Culture = _cultureManager.GetSiteCulture()
            };

            var patterns = settings.Patterns;
            patterns.Add(routePattern);
            settings.Patterns = patterns;

            // Define which pattern is the default.
            if (makeDefault || settings.Patterns.Count == 1)
            {
                settings.DefaultPatterns = new List<DefaultPattern> { new DefaultPattern { PatternIndex = "0", Culture = settings.Patterns[0].Culture } };
            }

            _contentDefinitionManager.AlterTypeDefinition(contentType, builder => builder.WithPart("AutoroutePart", settings.Build));
        }

        public IEnumerable<RoutePattern> GetPatterns(string contentType)
        {
            var settings = GetTypePartSettings(contentType).GetModel<AutorouteSettings>();
            return settings.Patterns;
        }

        public RoutePattern GetDefaultPattern(string contentType, string culture)
        {
            var settings = GetTypePartSettings(contentType).GetModel<AutorouteSettings>();
            var defaultPattern = settings.DefaultPatterns.FirstOrDefault(x => x.Culture == culture);
            var defaultPatternIndex = defaultPattern != null ? defaultPattern.PatternIndex : "0";

            if (String.IsNullOrWhiteSpace(defaultPatternIndex))
                defaultPatternIndex = "0";

            if (!settings.DefaultPatterns.Any(x => String.Equals(x.Culture, culture, StringComparison.OrdinalIgnoreCase)))
            {
                var patternIndex = String.IsNullOrWhiteSpace(settings.DefaultPatternIndex) ? "0" : settings.DefaultPatternIndex;
                // Lazy updating from old setting.
                if (String.Equals(culture, _cultureManager.GetSiteCulture(), StringComparison.OrdinalIgnoreCase))
                {
                    settings.DefaultPatterns.Add(new DefaultPattern { PatternIndex = patternIndex, Culture = culture });
                    return settings.Patterns.Where(x => x.Culture == null).ElementAt(Convert.ToInt32(defaultPatternIndex));
                }
                else {
                    settings.DefaultPatterns.Add(new DefaultPattern { PatternIndex = "0", Culture = culture });
                    return new RoutePattern { Name = "Title", Description = "my-title", Pattern = "{Content.Slug}", Culture = culture };
                }
            }

            // Return a default pattern if set.
            var patternCultureSearch = settings.Patterns.Any(x => String.Equals(x.Culture, culture, StringComparison.OrdinalIgnoreCase)) ? culture : null;

            if (settings.Patterns.Any())
            {
                if (settings.Patterns.Where(x => x.Culture == patternCultureSearch).ElementAt(Convert.ToInt32(defaultPatternIndex)) != null)
                {
                    return settings.Patterns.Where(x => x.Culture == patternCultureSearch).ElementAt(Convert.ToInt32(defaultPatternIndex));
                };
            }

            // Return a default pattern if none is defined.
            return new RoutePattern { Name = "Title", Description = "my-title", Pattern = "{Content.Slug}", Culture = culture };
        }

        private SettingsDictionary GetTypePartSettings(string contentType)
        {
            var contentDefinition = _contentDefinitionManager.GetTypeDefinition(contentType);

            if (contentDefinition == null)
            {
                throw new OrchardException(T("Unknown content type: {0}", contentType));
            }

            return contentDefinition.Parts.First(x => x.PartDefinition.Name == "AutoroutePart").Settings;
        }
    }
}
