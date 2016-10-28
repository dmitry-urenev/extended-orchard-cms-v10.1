using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents;
using Orchard.Core.Contents.Settings;
using Orchard.Core.Navigation.Models;
using Orchard.DisplayManagement;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Localization.ViewModels;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Html;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Orchard.Localization.Controllers
{
    [ValidateInput(false)]
    public class AdminController : Controller, IUpdateModel {
        private readonly IContentManager _contentManager;
        private readonly ICultureManager _cultureManager;
        private readonly ILocalizationService _localizationService;

        public AdminController(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            ICultureManager cultureManager,
            ILocalizationService localizationService,
            IShapeFactory shapeFactory) {
            _contentManager = contentManager;
            _cultureManager = cultureManager;
            _localizationService = localizationService;
            T = NullLocalizer.Instance;
            Services = orchardServices;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        private List<string> GetAvailableCultures(IContent contentItem)
        {
            var availableCultures = _cultureManager.ListCultures().ToList();
            var localizations = _localizationService.GetLocalizations(contentItem, VersionOptions.Latest)
                .Select(c =>
                {
                    var localized = c.ContentItem.As<LocalizationPart>();
                    if (localized.Culture == null)
                        localized.Culture = _cultureManager.GetCultureByName(_cultureManager.GetSiteCulture());
                    return c;
                }).ToList();

            localizations.ForEach(l => availableCultures.Remove(l.Culture.Culture));

            string masterCulture = _localizationService.GetContentCulture(contentItem);
            availableCultures.Remove(masterCulture);

            return availableCultures;
        }

        public ActionResult Translate(int id, string to)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.DraftRequired);
            if (contentItem == null)
                return HttpNotFound();

            var masterLocalizationPart = contentItem.As<LocalizationPart>();
            if (masterLocalizationPart == null)
                return HttpNotFound();

            // Check is current item stll exists, and redirect.
            var existingTranslation = _localizationService.GetLocalizedContentItem(contentItem, to);
            if (existingTranslation != null)
            {
                var existingTranslationMetadata = _contentManager.GetItemMetadata(existingTranslation);
                return RedirectToAction(
                    Convert.ToString(existingTranslationMetadata.EditorRouteValues["action"]),
                    existingTranslationMetadata.EditorRouteValues);
            }

            //var contentItemTranslation = _contentManager.New<LocalizationPart>(contentItem.ContentType);
            //contentItemTranslation.MasterContentItem = contentItem;

            //var content = _contentManager.BuildEditor(contentItemTranslation);
                              
            var availableCultures = GetAvailableCultures(contentItem);

            var selectedCulture = availableCultures.SingleOrDefault(s => string.Equals(s, to, StringComparison.OrdinalIgnoreCase))
                ?? _cultureManager.GetSiteCulture();

            if (!availableCultures.Contains(selectedCulture))
                selectedCulture = availableCultures.FirstOrDefault();

            var model = new AddLocalizationViewModel
            {
                Id = id,
                SiteCulture = _cultureManager.GetSiteCulture(),
                SelectedCulture = selectedCulture,
                AvailableCultures = availableCultures
            };

            PrepareItem(contentItem, selectedCulture);

            model.Content = _contentManager.BuildEditor(contentItem);

            // Cancel transaction so that the routepart is not modified.
            Services.TransactionManager.Cancel();

            model.OriginalContent = contentItem;
            return View(model);
        }

        public void PrepareItem(IContent contentItem, string selectedCulture)
        {
            // is used to hide the LocalizationPart that is included in current ContentType 
            if (contentItem.As<LocalizationPart>().Culture != null)
                contentItem.As<LocalizationPart>().Culture = null;

            // link parent translation to new translation of the content item, if parent exists
            if (contentItem.Has<ICommonPart>() && contentItem.As<ICommonPart>().Container != null)
            {
                var parent = contentItem.As<ICommonPart>().Container.ContentItem;
                var parentLocalization = _localizationService.GetLocalizations(parent, VersionOptions.Latest)
                    .Where(p => p.As<LocalizationPart>().Culture != null && p.As<LocalizationPart>().Culture.Culture == selectedCulture)
                    .FirstOrDefault();

                if (parentLocalization != null)
                {
                    contentItem.As<ICommonPart>().Container = parentLocalization.ContentItem;
                }
            }

            contentItem.As<LocalizationPart>().MasterContentItem = contentItem;
            if (contentItem.Has<AutoroutePart>())
            {
                var autoroutePart = contentItem.As<AutoroutePart>();
                autoroutePart.IsHomePage = false;
                autoroutePart.DisplayAlias = "";
            }
        }


        [HttpPost, ActionName("Translate")]
        [FormValueRequired("submit.ChangeLanguage")]
        public ActionResult Translate_ChangeLanguage(int id)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.DraftRequired);
            var original = _contentManager.Get(id, VersionOptions.DraftRequired);

            var model = new AddLocalizationViewModel();
            TryUpdateModel(model);

            var availableCultures = GetAvailableCultures(contentItem);
            var selectedCulture = model.SelectedCulture ?? _cultureManager.GetSiteCulture();

            if (!availableCultures.Contains(selectedCulture))
                selectedCulture = availableCultures.FirstOrDefault();

            model.Id = id;
            model.SiteCulture = _cultureManager.GetSiteCulture();
            model.SelectedCulture = selectedCulture;
            model.AvailableCultures = availableCultures;

            model.Content = _contentManager.UpdateEditor(contentItem, this);

            PrepareItem(contentItem, selectedCulture);

            model.Content = _contentManager.BuildEditor(contentItem);

            Services.TransactionManager.Cancel();

            model.OriginalContent = original;

            return View("Translate", model);
        }

        [HttpPost, ActionName("Translate")]
        [FormValueRequired("submit.Save")]
        public ActionResult TranslatePOST(int id, string returnUrl)
        {
            return TranslatePOST(id, returnUrl, contentItem =>
            {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    Services.ContentManager.Publish(contentItem);
            });
        }

        [HttpPost, ActionName("Translate")]
        [FormValueRequired("submit.Publish")]
        public ActionResult TranslateAndPublishPOST(int id, string returnUrl)
        {
            return TranslatePOST(id, returnUrl, contentItem => Services.ContentManager.Publish(contentItem));
        }

        public ActionResult TranslatePOST(int id, string returnUrl, Action<ContentItem> conditionallyPublish)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            if (contentItem == null)
                return HttpNotFound();

            var masterLocalizationPart = contentItem.As<LocalizationPart>();
            if (masterLocalizationPart == null)
                return HttpNotFound();

            var model = new AddLocalizationViewModel();
            TryUpdateModel(model);

            var existingTranslation = _localizationService.GetLocalizedContentItem(contentItem, model.SelectedCulture);
            if (existingTranslation != null)
            {
                var existingTranslationMetadata = _contentManager.GetItemMetadata(existingTranslation);
                return RedirectToAction(
                    Convert.ToString(existingTranslationMetadata.EditorRouteValues["action"]),
                    existingTranslationMetadata.EditorRouteValues);
            }

            var contentItemTranslation = _contentManager
                .Create<LocalizationPart>(contentItem.ContentType, VersionOptions.Draft, part =>
                {
                    part.MasterContentItem = contentItem;
                    if (!string.IsNullOrWhiteSpace(model.SelectedCulture))
                    {
                        part.Culture = _cultureManager.GetCultureByName(model.SelectedCulture);
                    }
                });

            model.Content = _contentManager.UpdateEditor(contentItemTranslation, this);

            if (contentItemTranslation.Has<ICommonPart>() && contentItem.Has<ICommonPart>()
                && contentItem.As<ICommonPart>().Container != null)
            {
                var parent = contentItem.As<ICommonPart>().Container.ContentItem;
                var parentLocalization = _localizationService.GetLocalizations(parent, VersionOptions.Latest)
                    .Where(p => p.As<LocalizationPart>().Culture != null && p.As<LocalizationPart>().Culture.Culture == model.SelectedCulture)
                    .FirstOrDefault();
                if (parentLocalization != null)
                {
                    contentItemTranslation.As<ICommonPart>().Container = parentLocalization.ContentItem;
                }
            }
            if (contentItemTranslation.Has<MenuPart>())
            {
                contentItemTranslation.As<MenuPart>().MenuPosition = contentItem.As<MenuPart>().MenuPosition;
            }

            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return Translate(model.Id, model.SelectedCulture);
            }

            conditionallyPublish(contentItemTranslation.ContentItem);

            Services.Notifier.Information(T("Created content item translation."));

            string previousRoute = null;

            if (contentItemTranslation.Has<IAliasAspect>()
                && !string.IsNullOrWhiteSpace(returnUrl)
                && Request.IsLocalUrl(returnUrl)
                // only if the original returnUrl is the content itself
                && String.Equals(returnUrl, Url.ItemDisplayUrl(contentItemTranslation), StringComparison.OrdinalIgnoreCase)
                )
            {
                previousRoute = contentItemTranslation.As<IAliasAspect>().Path;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl)
                && previousRoute != null
                && !String.Equals(contentItemTranslation.As<IAliasAspect>().Path, previousRoute, StringComparison.OrdinalIgnoreCase))
            {
                returnUrl = Url.ItemDisplayUrl(contentItemTranslation);
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.ItemEditUrl(contentItemTranslation);
            }
            return this.RedirectLocal(returnUrl);
        }

        [HttpPost]
        public ActionResult RemoveWithTranslations(int id, string returnUrl)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Permissions.DeleteContent, contentItem, T("Couldn't remove content")))
                return new HttpUnauthorizedResult();

            if (contentItem != null)
            {
                var translations = _localizationService.GetLocalizations(contentItem, VersionOptions.Latest);
                List<string> cultures = new List<string>();
                if (contentItem.Has<ILocalizableAspect>())
                    cultures.Add(contentItem.As<ILocalizableAspect>().Culture);

                _contentManager.Remove(contentItem);

                foreach (var t in translations)
                {
                    cultures.Add(t.As<ILocalizableAspect>().Culture);
                    _contentManager.Remove(t.ContentItem);
                }

                cultures = cultures.Where(c => !string.IsNullOrEmpty(c)).ToList();

                string msg = (string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                    ? T("That content has been removed.")
                    : T("That {0} has been removed.", contentItem.TypeDefinition.DisplayName))
                    + (cultures.Any() ? (" Removed translations: " + string.Join(", ", cultures)) : "");

                Services.Notifier.Information(T(msg));
            }

            return this.RedirectLocal(returnUrl, "~/Admin");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}