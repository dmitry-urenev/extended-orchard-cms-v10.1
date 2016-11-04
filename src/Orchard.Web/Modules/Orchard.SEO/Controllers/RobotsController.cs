using Orchard.Caching;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security.Permissions;
using Orchard.SEO.Services;
using Orchard.SEO.ViewModels;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Orchard.SEO.Controllers
{
    [OrchardFeature("Orchard.Robots")]
    public class RobotsController : Controller
    {
        private const string ContentType = "text/plain";

        private readonly IRobotsService _robotsService;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public RobotsController(IOrchardServices orchardServices, 
            IRobotsService robotsService, ICacheManager cacheManager, ISignals signals)
        {
            _robotsService = robotsService;
            _cacheManager = cacheManager;
            _signals = signals;

            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Index()
        {
            var content = _cacheManager.Get("RobotsFile.Settings",
                              ctx =>
                              {
                                  ctx.Monitor(_signals.When("RobotsFile.SettingsChanged"));
                                  var robotsFile = _robotsService.Get();
                                  return robotsFile.FileContent;
                              });
            if (string.IsNullOrWhiteSpace(content))
            {
                content = _robotsService.Get().FileContent;
            }
            return new ContentResult()
            {
                ContentType = ContentType,
                ContentEncoding = Encoding.UTF8,
                Content = content
            };
        }

        [Admin]
        public ActionResult Edit()
        {
            if (!Authorized(Permissions.ConfigureRobotsTextFile))
                return new HttpUnauthorizedResult();
            return View(new TextFileViewModel() { Text = _robotsService.Get().FileContent });
        }

        [Admin, HttpPost, ValidateInput(false)]
        public ActionResult Edit(TextFileViewModel viewModel)
        {
            if (!Authorized(Permissions.ConfigureRobotsTextFile))
                return new HttpUnauthorizedResult();
            var saveResult = _robotsService.Save(viewModel.Text);
            if (saveResult.Item1)
                Services.Notifier.Information(T("Robots.txt settings successfully saved"));
            else
            {
                Services.Notifier.Information(T("Robots.txt saved with warnings"));
                saveResult.Item2.ToList().ForEach(error =>
                    Services.Notifier.Warning(T(error))
                );
            }
            return View(viewModel);
        }

        private bool Authorized(Permission permission)
        {
            return Services.Authorizer.Authorize(permission, T("Cannot manage robots.txt file"));
        }

    }
}