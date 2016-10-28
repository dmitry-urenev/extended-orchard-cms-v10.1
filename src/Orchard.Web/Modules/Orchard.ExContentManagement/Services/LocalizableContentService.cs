using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.Localization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Orchard.Localization.Services;

namespace Orchard.ExContentManagement
{
    public class LocalizableContentService : Orchard.ExContentManagement.ILocalizableContentService
    {
        private IOrchardServices _orchardServices;
        private IAliasService _aliasService;
        private ICultureManager _cultureManager;
        private IContentManager _contentManager;
        private ILocaliza_localizableContentService;

        public LocalizableContentService(
            IAliasService aliasService,
            IContentManager contentManager,
            IOrchardServices orchardServices,
            ICultureManager cultureManager,
            Orchard.CultureSwitcher.Services.ILocalizableContentService localizableContentService)
        {
            _aliasService = aliasService;
            _contentManager = contentManager;
            _orchardServices = orchardServices;
            _cultureManager = cultureManager;
            _localizableContentService = localizableContentService;
        }

        public string ResolveLocalizedUrl(string relativeUrl, string culture = null)
        {
            string path = relativeUrl;

            if (!string.IsNullOrEmpty(path))
            {
                path = path.TrimStart('~', '/');
            }
            RouteValueDictionary routeValueDictionary = _aliasService.Get(path ?? "");

            if (routeValueDictionary != null && routeValueDictionary.ContainsKey("Id"))
            {
                int routeId = Convert.ToInt32(routeValueDictionary["Id"]);
                ContentItem content = _orchardServices.ContentManager.Get(routeId, VersionOptions.Published);
                if (content == null)
                {
                    return relativeUrl;
                }

                var workContext = _orchardServices.WorkContext;
                var httpContext = workContext != null ? workContext.HttpContext : null;

                if (httpContext != null)
                {
                    string currentCultureName = string.IsNullOrEmpty(culture) ? _cultureManager.GetCurrentCulture(httpContext) : culture;
                    string localizedPath;

                    //content may not have localized version and we use "Try" approach
                    if (_localizableContentService.TryFindLocalizedPath(content, currentCultureName, null, out localizedPath))
                    {
                        string returnUrl = localizedPath;

                        if (!returnUrl.StartsWith("~/"))
                        {
                            returnUrl = "~/" + returnUrl;
                        }

                        return returnUrl;
                    }
                }
            }
            return relativeUrl;
        }
    }
}