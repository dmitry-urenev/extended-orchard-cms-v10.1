using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization.Services;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Themes;
using System.Web;
using System.Web.Mvc;
                                    
using Orchard.Core.Contents.PageContext;

namespace Orchard.CultureSwitcher.Controllers
{
    [HandleError]
    public class LocalizableHomeController : Controller
    {
        private readonly IHomeAliasService _homeAliasService;
        private readonly ICultureManager _cultureManager;
        private readonly ILocalizationService _localizationService;
        private readonly IOrchardServices _orchardServices;

        public LocalizableHomeController(IOrchardServices orchardServices,
            ICultureManager cultureManager,
            ILocalizationService localizationService,
            IHomeAliasService homeAliasService)
        {
            _orchardServices = orchardServices;
            _cultureManager = cultureManager;
            _localizationService = localizationService;
            _homeAliasService = homeAliasService;
        }

        public ILogger Logger { get; set; }

        [Themed]
        public ActionResult Index()
        {
            var homePage = _homeAliasService.GetHomePage();
            if (homePage == null)
                return new HttpNotFoundResult();

            HttpContextBase httpContext = _orchardServices.WorkContext.HttpContext;
            string currentCultureName = _cultureManager.GetCurrentCulture(httpContext);

            if (homePage.Has<ILocalizableAspect>())
            {
                string hCulture = homePage.As<ILocalizableAspect>().Culture;
                if (string.IsNullOrEmpty(hCulture))
                    hCulture = _cultureManager.GetSiteCulture();

                if (!currentCultureName.Equals(hCulture))
                {
                    string localizedHomeUrl = _localizationService.GetLocalizedPath(homePage, currentCultureName);
                    if (!string.IsNullOrEmpty(localizedHomeUrl))
                    {
                        if (!localizedHomeUrl.StartsWith("~/"))
                            localizedHomeUrl = "~/" + localizedHomeUrl;

                        return this.RedirectLocal(localizedHomeUrl);
                    }
                }
            }

            var pcontext = _orchardServices.WorkContext.GetPageContext();
            pcontext.ContentItem = homePage.ContentItem;

            dynamic model = _orchardServices.ContentManager.BuildDisplay(homePage);
            return new ShapeResult(this, model);
        }
    }
}