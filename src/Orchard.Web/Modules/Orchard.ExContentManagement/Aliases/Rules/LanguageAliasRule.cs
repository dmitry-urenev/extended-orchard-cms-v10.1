using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ExContentManagement.Models;
using Orchard.Localization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Aliases.Rules
{
    public class LanguageAliasRule : IAliasRule
    {
        private readonly ICultureManager _cultureManager;

        public LanguageAliasRule(ICultureManager cultureManager)
        {
            _cultureManager = cultureManager;
        }

        public bool Match(IContent contentItem)
        {
            if (contentItem.Has<ILocalizableAspect>() &&
                   (!contentItem.Has<ParentContentPart>() || contentItem.Has<ParentContentPart>() && contentItem.As<ParentContentPart>().ParentContent == null))
            {
                return true;
            }
            return false;
        }

        public void Apply(IContent contentItem, AliasBuildContext context)
        {
            string alias = context.Alias;

            if (contentItem.Has<ILocalizableAspect>() &&
                   (!contentItem.Has<ParentContentPart>() || contentItem.Has<ParentContentPart>() && contentItem.As<ParentContentPart>().ParentContent == null))
            {
                var siteCulture = _cultureManager.GetSiteCulture();
                var avalableCultures = _cultureManager.ListCultures();

                alias = contentItem.As<IAliasAspect>().Path;
                var segments = alias.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0)
                {
                    var languageSegment = segments[0];

                    if (avalableCultures.Contains(languageSegment, StringComparer.OrdinalIgnoreCase))
                    {
                        alias = alias.Substring(alias.IndexOf("/") + 1); // remove lang prefix
                    }
                }

                context.Segments.Set("{Slug}", alias);

                string currentCulture = contentItem.As<ILocalizableAspect>().Culture;
                if (!string.IsNullOrEmpty(currentCulture) && !string.Equals(currentCulture, siteCulture, StringComparison.OrdinalIgnoreCase))
                {
                    var culture = currentCulture.ToLower();
                    context.Segments.Set("{Culture}", culture);
                    alias = culture + "/" + alias; // de-de/page1
                }
            }
            context.Alias = alias;
        }

        public string Rule
        {
            get
            {
                return "{Culture}/{Slug}";
            }
        }

        public int Priority
        {
            get { return 10; }
        }
    }
}