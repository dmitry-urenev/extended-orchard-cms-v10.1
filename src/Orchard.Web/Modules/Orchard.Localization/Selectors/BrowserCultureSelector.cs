using System;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;

namespace Orchard.Localization.Selectors
{
    public class BrowserCultureSelector : ICultureSelector
    {
        private readonly Lazy<ICultureManager> _cultureManager;

        public BrowserCultureSelector(
            Lazy<ICultureManager> cultureManager)
        {
            _cultureManager = cultureManager;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            if (context == null) return null;

            /* Fall back to Browser */
            var userLanguages = context.Request.UserLanguages;

            if (userLanguages == null || !userLanguages.Any())
                return null;

            var cultures = _cultureManager.Value.ListCultures().ToList();

            foreach (var userLanguage in userLanguages
                .Select(ul => ul.Split(';')[0].Trim()))
            {
                if (cultures.Contains(userLanguage, StringComparer.OrdinalIgnoreCase))
                {
                    if ("~/".Equals(context.Request.AppRelativeCurrentExecutionFilePath))
                    {
                        var hasCultureCookie = context.Request.Cookies.AllKeys
                            .Any(k => CookieCultureSelector.FrontEndCookieName.Equals(k, StringComparison.OrdinalIgnoreCase));

                        if (hasCultureCookie)
                        {
                            return new CultureSelectorResult { Priority = -4, CultureName = CultureInfo.CreateSpecificCulture(userLanguage).Name };
                        }
                        else
                        {
                            return new CultureSelectorResult { Priority = 0, CultureName = CultureInfo.CreateSpecificCulture(userLanguage).Name };
                        }
                    }

                }
            }

            return null;
        }
    }
}