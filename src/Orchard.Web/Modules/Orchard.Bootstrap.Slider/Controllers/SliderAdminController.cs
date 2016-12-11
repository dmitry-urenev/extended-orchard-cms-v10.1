using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Settings;
using Orchard.Core.Contents;
using Orchard.Bootstrap.Slider.Models;
using Orchard.Core.Common.Models;
using Orchard.Bootstrap.Slider.Services;
using Orchard.Localization.Services;

namespace Orchard.Bootstrap.Slider.Controllers {

    [ValidateInput(false), Admin]
    public class SliderAdminController : Controller, IUpdateModel {
        private readonly ISliderService _sliderService;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        private readonly ICultureManager _cultureManager;

        public SliderAdminController(
            IOrchardServices services,
            ISliderService sliderService,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            ICultureManager cultureManager) {
            
            Services = services;

            _sliderService = sliderService;
            _contentManager = contentManager;
            _transactionManager = transactionManager;
            _siteService = siteService;
            _cultureManager = cultureManager;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;

        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult Create()
        {

            if (!Services.Authorizer.Authorize(Permissions.EditContent, T("Not allowed to edit content")))
                return new HttpUnauthorizedResult();

            SliderPart slider = Services.ContentManager.New<SliderPart>("Slider");
            if (slider == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(slider);
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContent, T("Couldn't create slider")))
                return new HttpUnauthorizedResult();

            var slider = Services.ContentManager.New<SliderPart>("Slider");

            _contentManager.Create(slider, VersionOptions.Draft);
            var model = _contentManager.UpdateEditor(slider, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                return View(model);
            }

            _contentManager.Publish(slider.ContentItem);
            return RedirectToAction("Items", new { sliderId = slider.Id });
        }

        public ActionResult Edit(int sliderId)
        {
            var slider = _contentManager.Get(sliderId, VersionOptions.Latest);

            if (slider == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditContent, slider, T("Not allowed to edit slider")))
                return new HttpUnauthorizedResult();

            var model = Services.ContentManager.BuildEditor(slider);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Delete")]
        public ActionResult EditDeletePOST(int sliderId)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContent, T("Couldn't delete slider")))
                return new HttpUnauthorizedResult();

            var slider = _contentManager.Get(sliderId, VersionOptions.DraftRequired);
            if (slider == null)
                return HttpNotFound();

            _contentManager.Remove(slider);

            Services.Notifier.Information(T("Slider deleted"));

            return RedirectToAction("List");
        }


        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int sliderId)
        {
            var slider = _contentManager.Get(sliderId, VersionOptions.DraftRequired);

            if (!Services.Authorizer.Authorize(Permissions.EditContent, slider, T("Couldn't edit slider")))
                return new HttpUnauthorizedResult();

            if (slider == null)
                return HttpNotFound();

            var model = Services.ContentManager.UpdateEditor(slider, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(model);
            }

            _contentManager.Publish(slider);
            Services.Notifier.Information(T("Slider information updated"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Remove(int sliderId)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContent, T("Couldn't delete slider")))
                return new HttpUnauthorizedResult();

            var slider = _contentManager.Get(sliderId, VersionOptions.Latest);

            if (slider == null)
                return HttpNotFound();

            _contentManager.Remove(slider);

            Services.Notifier.Information(T("Slider was successfully deleted"));
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var list = Services.New.List();
            list.AddRange(_sliderService.Get(VersionOptions.Latest)
                .Where(x => Services.Authorizer.Authorize(Permissions.MetaListContent, x))
                .Select(p => Services.ContentManager.BuildDisplay(p, "SummaryAdmin")));

            var viewModel = Services.New.ViewModel()
                .ContentItems(list);

            return View(viewModel);
        }


        public ActionResult Items(int sliderId, PagerParameters pagerParameters, string lang = null)
        {
            var availableCultures = _cultureManager.ListCultures();
            
            if (string.IsNullOrEmpty(lang) || !availableCultures.Contains(lang))
                lang = _cultureManager.GetSiteCulture();

            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            SliderPart sliderPart = _sliderService.Get(sliderId, VersionOptions.Latest);

            if (sliderPart == null)
                return HttpNotFound();

            var sliderItems = _sliderService.List(sliderPart, lang, pager.GetStartIndex(), pager.PageSize, VersionOptions.Latest).ToArray();
            var sliderItemsShapes = sliderItems.Select(bp => _contentManager.BuildDisplay(bp, "SummaryAdmin")).ToArray();


            var list = Shape.List();
            list.AddRange(sliderItemsShapes);

            var totalItemCount = _sliderService.ItemsCount(sliderPart, lang, VersionOptions.Latest);

            var viewModel = Services.New.ViewModel()
                .AvailableCultures(availableCultures)
                .CurrentCulture(lang)
                .Slider(sliderPart)
                .ContentItems(list)
                .Pager(Shape.Pager(pager).TotalItemCount(totalItemCount));

            return View(viewModel);
        }


        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}