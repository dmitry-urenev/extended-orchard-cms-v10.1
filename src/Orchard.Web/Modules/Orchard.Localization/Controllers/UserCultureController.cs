using System;
using System.Web;
using System.Web.Mvc;
using Orchard.Autoroute.Models;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using System.Web.Routing;
using Orchard.Localization.Services;
using Orchard.Localization.Providers;

namespace Orchard.CultureSwitcher.Controllers
{
    public class UserCultureController : Controller
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICultureStorageProvider _cultureStorageProvider;

        public UserCultureController(
            IOrchardServices services,
            ILocalizationService localizationService,
            ICultureStorageProvider cultureStorageProvider)
        {
            Services = services;

            _localizationService = localizationService;
            _cultureStorageProvider = cultureStorageProvider;
        }

        public IOrchardServices Services { get; set; }

        public ActionResult ChangeCulture(string cultureName, string returnUrl)
        {
            if (string.IsNullOrEmpty(cultureName))
            {
                throw new ArgumentNullException(cultureName);
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                var request = Services.WorkContext.HttpContext.Request;
                if (request.UrlReferrer != null)
                {
                    string localUrl = GetAppRelativePath(request.UrlReferrer.AbsolutePath, request);
                    //support for pre-encoded Unicode urls
                    returnUrl = HttpUtility.UrlDecode(localUrl);
                }
            }

            returnUrl = _localizationService.GetLocalizedPath(returnUrl, cultureName);

            _cultureStorageProvider.SetCulture(cultureName);

            if (!returnUrl.StartsWith("~/"))
            {
                returnUrl = "~/" + returnUrl;
            }

            return this.RedirectLocal(returnUrl);
        }

        private string GetAppRelativePath(string logicalPath, HttpRequestBase request)
        {
            if (request.ApplicationPath == null)
            {
                return String.Empty;
            }

            logicalPath = logicalPath.ToLower();
            string appPath = request.ApplicationPath.ToLower();
            if (appPath != "/")
            {
                appPath += "/";
            }
            else
            {
                // Root web relative path is empty
                return logicalPath.Substring(1);
            }

            return logicalPath.Replace(appPath, "");
        }
    }
}