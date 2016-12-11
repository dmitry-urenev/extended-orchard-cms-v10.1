using System;
using System.Reflection;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents.Settings;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Mvc.AntiForgery;
using Orchard.Mvc.Extensions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard.Bootstrap.Slider.Services;
using Orchard.Bootstrap.Slider.Models;
using Orchard.Core.Contents;

namespace Orchard.Bootstrap.Slider.Controllers {

    /// <summary>
    /// TODO: (PH:Autoroute) This replicates a whole lot of Core.Contents functionality. All we actually need to do is take the SliderId from the query string in the SliderItemPartDriver, and remove
    /// helper extensions from UrlHelperExtensions.
    /// </summary>
    [ValidateInput(false), Admin]
    public class SliderItemAdminController : Controller, IUpdateModel {
        private readonly ISliderService _sliderService;
        private readonly ISliderItemService _sliderItemService;

        public SliderItemAdminController(IOrchardServices services, ISliderService sliderService, ISliderItemService sliderItemService) {
            Services = services;
            _sliderService = sliderService;
            _sliderItemService = sliderItemService;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Create(int sliderId) {

            var slider = _sliderService.Get(sliderId, VersionOptions.Latest).As<SliderPart>();
            if (slider == null)
                return HttpNotFound();

            var sliderItem = Services.ContentManager.New<SliderItemPart>("SliderItem");
            sliderItem.SliderPart = slider;

            if (!Services.Authorizer.Authorize(Permissions.EditContent, slider, T("Not allowed to create slider item")))
                return new HttpUnauthorizedResult();

            var model = Services.ContentManager.BuildEditor(sliderItem);
            
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePOST(int sliderId, string returnUrl)
        {
            return CreatePOST(sliderId, false, returnUrl);
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishPOST(int sliderId, string returnUrl)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublishOwnContent, T("Couldn't create content")))
                return new HttpUnauthorizedResult();

            return CreatePOST(sliderId, true, returnUrl);
        }

        private ActionResult CreatePOST(int sliderId, bool publish = false, string returnUrl = null)
        {
            var slider = _sliderService.Get(sliderId, VersionOptions.Latest).As<SliderPart>();

            if (slider == null)
                return HttpNotFound();

            var sliderItem = Services.ContentManager.New<SliderItemPart>("SliderItem");
            sliderItem.SliderPart = slider;

            if (!Services.Authorizer.Authorize(Permissions.EditContent, slider, T("Couldn't create slider item")))
                return new HttpUnauthorizedResult();

            Services.ContentManager.Create(sliderItem, VersionOptions.Draft);
            var model = Services.ContentManager.UpdateEditor(sliderItem, this);

            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(model);
            }

            if (publish)
            {
                if (!Services.Authorizer.Authorize(Permissions.PublishContent, slider.ContentItem, T("Couldn't publish slider item")))
                    return new HttpUnauthorizedResult();

                Services.ContentManager.Publish(sliderItem.ContentItem);
            }

            Services.Notifier.Information(T("Your {0} has been created.", sliderItem.TypeDefinition.DisplayName));

            return this.RedirectLocal(returnUrl,
                Url.Action("Items", "SliderAdmin", new { sliderId = sliderId, area = "Orchard.Bootstrap.Slider" }));
        }

        //todo: the content shape template has extra bits that the core contents module does not (remove draft functionality)
        //todo: - move this extra functionality there or somewhere else that's appropriate?
        public ActionResult Edit(int sliderId, int itemId) {
            var slider = _sliderService.Get(sliderId, VersionOptions.Latest);
            if (slider == null)
                return HttpNotFound();

            var item = _sliderItemService.Get(itemId, VersionOptions.Latest);
            if (item == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditContent, item, T("Couldn't edit slider item")))
                return new HttpUnauthorizedResult();

            var model = Services.ContentManager.BuildEditor(item);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int sliderId, int itemId, string returnUrl) {
            return EditPOST(sliderId, itemId, returnUrl, contentItem => {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    Services.ContentManager.Publish(contentItem);
            });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditAndPublishPOST(int sliderId, int itemId, string returnUrl) {
            var slider = _sliderService.Get(sliderId, VersionOptions.Latest);
            if (slider == null)
                return HttpNotFound();

            // Get draft (create a new version if needed)
            var sliderItem = _sliderItemService.Get(itemId, VersionOptions.DraftRequired);
            if (sliderItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.PublishContent, sliderItem, T("Couldn't publish slider item")))
                return new HttpUnauthorizedResult();

            return EditPOST(sliderId, itemId, returnUrl, contentItem => Services.ContentManager.Publish(contentItem));
        }

        public ActionResult EditPOST(int sliderId, int itemId, string returnUrl, Action<ContentItem> conditionallyPublish)
        {
            var slider = _sliderService.Get(sliderId, VersionOptions.Latest);
            if (slider == null)
                return HttpNotFound();

            // Get draft (create a new version if needed)
            var sliderItem = _sliderItemService.Get(itemId, VersionOptions.DraftRequired);
            if (sliderItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditContent, sliderItem, T("Couldn't edit slider item")))
                return new HttpUnauthorizedResult();

            // Validate form input
            var model = Services.ContentManager.UpdateEditor(sliderItem, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(model);
            }

            conditionallyPublish(sliderItem.ContentItem);

            Services.Notifier.Information(T("Your {0} has been saved.", sliderItem.TypeDefinition.DisplayName));

            return this.RedirectLocal(returnUrl,
                Url.Action("Items", "SliderAdmin", new { sliderId = sliderId, area = "Orchard.Bootstrap.Slider" }));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult DiscardDraft(int id) {
            // get the current draft version
            var draft = Services.ContentManager.Get(id, VersionOptions.Draft);
            if (draft == null) {
                Services.Notifier.Information(T("There is no draft to discard."));
                return RedirectToEdit(id);
            }

            // check edit permission
            if (!Services.Authorizer.Authorize(Permissions.EditContent, draft, T("Couldn't discard slider item draft")))
                return new HttpUnauthorizedResult();

            // locate the published revision to revert onto
            var published = Services.ContentManager.Get(id, VersionOptions.Published);
            if (published == null) {
                Services.Notifier.Information(T("Can not discard draft on unpublished slider item."));
                return RedirectToEdit(draft);
            }

            // marking the previously published version as the latest
            // has the effect of discarding the draft but keeping the history
            draft.VersionRecord.Latest = false;
            published.VersionRecord.Latest = true;

            Services.Notifier.Information(T("Slider item draft version discarded"));
            return RedirectToEdit(published);
        }

        ActionResult RedirectToEdit(int id) {
            return RedirectToEdit(Services.ContentManager.GetLatest<SliderItemPart>(id));
        }

        ActionResult RedirectToEdit(IContent item) {
            if (item == null || item.As<SliderItemPart>() == null)
                return HttpNotFound();
            return RedirectToAction("Edit", new { SliderId = item.As<SliderItemPart>().SliderPart.Id, PostId = item.ContentItem.Id });
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Delete(int sliderId, int itemId, string returnUrl)
        {
            //refactoring: test PublishContent/PublishContent in addition if published

            var slider = _sliderService.Get(sliderId, VersionOptions.Latest);
            if (slider == null)
                return HttpNotFound();

            var item = _sliderItemService.Get(itemId, VersionOptions.Latest);
            if (item == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.DeleteContent, item, T("Couldn't delete slider item")))
                return new HttpUnauthorizedResult();

            _sliderItemService.Delete(item);
            Services.Notifier.Information(T("Slider item was successfully deleted"));

            return this.RedirectLocal(returnUrl,
                Url.Action("Items", "SliderAdmin", new { sliderId = sliderId, area = "Orchard.Bootstrap.Slider" }));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Publish(int sliderId, int itemId, string returnUrl)
        {
            var slider = _sliderService.Get(sliderId, VersionOptions.Latest);
            if (slider == null)
                return HttpNotFound();

            var item = _sliderItemService.Get(itemId, VersionOptions.Latest);
            if (item == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.PublishContent, item, T("Couldn't publish slider item")))
                return new HttpUnauthorizedResult();

            _sliderItemService.Publish(item);
            Services.Notifier.Information(T("Slider item successfully published."));

            return this.RedirectLocal(returnUrl,
				Url.Action("Items", "SliderAdmin", new { sliderId = sliderId, area = "Orchard.Bootstrap.Slider" }));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Unpublish(int sliderId, int itemId, string returnUrl)
        {
            var slider = _sliderService.Get(sliderId, VersionOptions.Latest);
            if (slider == null)
                return HttpNotFound();

            var item = _sliderItemService.Get(itemId, VersionOptions.Latest);
            if (item == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.PublishContent, item, T("Couldn't unpublish slider item")))
                return new HttpUnauthorizedResult();

            _sliderItemService.Unpublish(item);
            Services.Notifier.Information(T("Slider item successfully unpublished."));

            return this.RedirectLocal(returnUrl,
				Url.Action("Items", "SliderAdmin", new { sliderId = sliderId, area = "Orchard.Bootstrap.Slider" }));
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}