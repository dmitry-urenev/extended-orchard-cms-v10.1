using System.Text;
using System.Web.Mvc;
using Orchard.SEO.Services;
using Orchard.Caching;
using Orchard.Environment.Extensions;
using Orchard.Security.Permissions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard.SEO.ViewModels;
using Orchard.Localization;

namespace Orchard.SEO.Controllers
{
    [OrchardFeature("Orchard.Sitemap")]
    public class SitemapController : Controller
    {
        private const string ContentType = "text/xml";

        private readonly ISitemapService _sitemapService;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public SitemapController(IOrchardServices orchardServices,
            ISitemapService sitemapService, ICacheManager cacheManager, ISignals signals)
        {
            _sitemapService = sitemapService;
            _cacheManager = cacheManager;
            _signals = signals;

            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }


        public ActionResult Index()
        {
            var content = _cacheManager.Get("SitemapFile.Settings",
                              ctx =>
                              {
                                  ctx.Monitor(_signals.When("SitemapFile.SettingsChanged"));
                                  var fileRecord = _sitemapService.Get();
                                  return fileRecord.FileContent;
                              });
            if (string.IsNullOrWhiteSpace(content))
            {
                content = _sitemapService.Get().FileContent;
            }
            return new ContentResult()
            {
                ContentType = ContentType,
                ContentEncoding = Encoding.UTF8,
                Content = content
            };
        }

        private bool Authorized(Permission permission)
        {
            return Services.Authorizer.Authorize(permission, T("Cannot manage Sitemap.xml file"));
        }

        [Admin]
        public ActionResult Edit()
        {
            if (!Authorized(Permissions.ConfigureSitemapFile))
                return new HttpUnauthorizedResult();

            return View(new TextFileViewModel() { Text = _sitemapService.Get().FileContent });
        }

        [Admin, HttpPost, ValidateInput(false)]
        public ActionResult Edit(TextFileViewModel viewModel)
        {
            if (!Authorized(Permissions.ConfigureSitemapFile))
                return new HttpUnauthorizedResult();

            var valid = _sitemapService.Save(viewModel.Text);
            Services.Notifier.Information(T("Sitemap.xml was successfully saved"));

            return View(viewModel);
        }
    }
}