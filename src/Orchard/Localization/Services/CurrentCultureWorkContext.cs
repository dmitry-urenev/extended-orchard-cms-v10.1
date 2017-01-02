using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Mvc;
using Orchard.Caching;

namespace Orchard.Localization.Services {
    public class CurrentCultureWorkContext : IWorkContextStateProvider {
        private readonly IEnumerable<ICultureSelector> _cultureSelectors;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISignals _signals;
        private readonly ICacheManager _cacheManager;

        public CurrentCultureWorkContext(IEnumerable<ICultureSelector> cultureSelectors,
                IHttpContextAccessor httpContextAccessor,
                ISignals signals, ICacheManager cacheManager)
        {
            _cultureSelectors = cultureSelectors;
            _httpContextAccessor = httpContextAccessor;
            _signals = signals;
            _cacheManager = cacheManager;
        }

        public Func<WorkContext, T> Get<T>(string name) {
            if (name == "CurrentCulture") {
                return ctx => {

                    return (T)(object) _cacheManager.Get("CurrentCulture", true, context => {
                        context.Monitor(_signals.When(Localization.Signals.CurrentCultureChanged));

                        return GetCurrentCulture();
                    });

                };
            }
            return null;
        }

        private string GetCurrentCulture() {
            var httpContext = _httpContextAccessor.Current();

            var culture = _cultureSelectors
                .Select(c => c.GetCulture(httpContext))
                .Where(c => c != null)
                .OrderByDescending(c => c.Priority)
                .FirstOrDefault(c => !String.IsNullOrEmpty(c.CultureName));

            return culture == null ? String.Empty : culture.CultureName;
        }
    }
}
