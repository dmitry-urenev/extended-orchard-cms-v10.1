using System;
using System.Web;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Localization.Providers;
using Orchard.Localization.Services;
using Orchard.Mvc;
using Orchard.Services;
using Orchard.Caching;

namespace Orchard.Localization.Selectors
{
    public class CookieCultureSelector : ICultureSelector, ICultureStorageProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClock _clock;
        private readonly ShellSettings _shellSettings;
        private readonly ISignals _signals;

        internal const string FrontEndCookieName = "OrchardCurrentCulture-FrontEnd";
        internal const string AdminCookieName = "OrchardCurrentCulture-Admin";
        private const int DefaultExpireTimeYear = 1;

        public CookieCultureSelector(IHttpContextAccessor httpContextAccessor,
            IClock clock, 
            ISignals signals,
            ShellSettings shellSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _clock = clock;
            _shellSettings = shellSettings;
            _signals = signals;
        }

        public void SetCulture(string culture)
        {
            var httpContext = _httpContextAccessor.Current();

            if (httpContext == null) return;

            var cookieName = ContextHelpers.IsRequestAdmin(httpContext) ? AdminCookieName : FrontEndCookieName;

            var cookie = new HttpCookie(cookieName, culture)
            {
                Expires = _clock.UtcNow.AddYears(DefaultExpireTimeYear),
            };

            cookie.Domain = !httpContext.Request.IsLocal ? httpContext.Request.Url.Host : null;

            if (!String.IsNullOrEmpty(_shellSettings.RequestUrlPrefix))
            {
                cookie.Path = GetCookiePath(httpContext);
            }

            httpContext.Request.Cookies.Remove(cookieName);
            httpContext.Response.Cookies.Remove(cookieName);
            httpContext.Response.Cookies.Add(cookie);

            _signals.Trigger(Localization.Signals.CurrentCultureChanged);
        }

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            if (context == null) return null;

            var cookieName = ContextHelpers.IsRequestAdmin(context) ? AdminCookieName : FrontEndCookieName;

            var cookie = context.Request.Cookies.Get(cookieName);

            if (cookie != null)
                return new CultureSelectorResult { Priority = -2, CultureName = cookie.Value };

            return null;
        }

        private string GetCookiePath(HttpContextBase httpContext)
        {
            var cookiePath = httpContext.Request.ApplicationPath;
            if (cookiePath != null && cookiePath.Length > 1)
            {
                cookiePath += '/';
            }

            cookiePath += _shellSettings.RequestUrlPrefix;

            return cookiePath;
        }
    }
}