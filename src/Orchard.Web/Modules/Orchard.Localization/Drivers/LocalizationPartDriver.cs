using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Localization.ViewModels;

namespace Orchard.Localization.Drivers
{
    public class LocalizationPartDriver : ContentPartDriver<LocalizationPart>
    {
        private const string TemplatePrefix = "Localization";
        private readonly ICultureManager _cultureManager;
        private readonly ILocalizationService _localizationService;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;

        public LocalizationPartDriver(ICultureManager cultureManager,
            ILocalizationService localizationService,
            IContentManager contentManager,
            IOrchardServices orchardServices)
        {
            _cultureManager = cultureManager;
            _localizationService = localizationService;
            _contentManager = contentManager;
            _orchardServices = orchardServices;
        }

        protected override DriverResult Display(LocalizationPart part, string displayType, dynamic shapeHelper)
        {
            var availableCultures = _cultureManager.ListCultures().ToList();
            bool IsMultyLanguageSite = availableCultures.Count() > 1;

            if (IsMultyLanguageSite)
            {
                var masterId = part.HasTranslationGroup
                               ? part.Record.MasterContentItemId
                               : part.Id;

                var exisingLocalizations = GetEditorLocalizations(part).ToList();
                bool allowAddTranslations = part.ContentItem.Id == masterId && part.ContentItem.Id != 0;

                if (allowAddTranslations)
                {
                    exisingLocalizations.ForEach(l =>
                    {
                        if (l.Culture != null)
                            availableCultures.Remove(l.Culture.Culture);
                    });

                    string masterCulture = _localizationService.GetContentCulture(part.MasterContentItem ?? part.ContentItem);
                    availableCultures.Remove(masterCulture);
                    allowAddTranslations = availableCultures.Count > 0;
                }
                string currentCulture = part.Culture != null ? part.Culture.Culture : _cultureManager.GetSiteCulture();
                var context = _orchardServices.WorkContext;

                string selectedAdminCulture = context != null && context.HttpContext != null ? context.HttpContext.Request.QueryString["lang"] : "";

                return Combined(
                    ContentShape("Parts_Localization_ContentTranslations",
                        () => shapeHelper.Parts_Localization_ContentTranslations(MasterId: masterId, Culture: currentCulture, Localizations: exisingLocalizations, AllowAddTranslations: allowAddTranslations)),
                    ContentShape("Parts_Localization_ContentTranslations_Summary",
                        () => shapeHelper.Parts_Localization_ContentTranslations_Summary(MasterId: masterId, Culture: currentCulture, Localizations: exisingLocalizations,
                                AllowAddTranslations: allowAddTranslations, TargetCulture: selectedAdminCulture)),
                    ContentShape("Parts_Localization_ContentTranslations_SummaryAdmin",
                        () => shapeHelper.Parts_Localization_ContentTranslations_SummaryAdmin(MasterId: masterId, Culture: currentCulture, Localizations: exisingLocalizations,
                                AllowAddTranslations: allowAddTranslations, TargetCulture: selectedAdminCulture)),
                    ContentShape("Parts_Localization_ContentTranslations_NavigationAdmin",
                        () => shapeHelper.Parts_Localization_ContentTranslations_NavigationAdmin(MasterId: masterId, Culture: currentCulture, Localizations: exisingLocalizations,
                                AllowAddTranslations: allowAddTranslations, TargetCulture: selectedAdminCulture)),
                    ContentShape("Parts_Localization_DeleteWithTranslations_SummaryAdmin",
                        () => shapeHelper.Parts_Localization_DeleteWithTranslations_SummaryAdmin())
                    );
            }
            else
            {
                return null;
            }
        }

        protected override DriverResult Editor(LocalizationPart part, dynamic shapeHelper)
        {
            var availableCultures = _cultureManager.ListCultures().ToList();
            bool IsMultyLanguageSite = availableCultures.Count() > 1;

            var context = _orchardServices.WorkContext;

            if (IsMultyLanguageSite)
            {
                var localizations = GetEditorLocalizations(part).ToList();

                string selectedCulture = GetCulture(part);
                if (selectedCulture == null && part.ContentItem.Id == 0)
                {
                    string selectedAdminCulture = context != null && context.HttpContext != null ? context.HttpContext.Request.QueryString["lang"] : "";
                    if (!string.IsNullOrEmpty(selectedAdminCulture))
                        selectedCulture = selectedAdminCulture;
                    else
                        selectedCulture = _cultureManager.GetSiteCulture();
                }

                localizations.ForEach(l => availableCultures.Remove(l.Culture.Culture));
                if (localizations.Count > 0)
                {
                    string masterCulture = _localizationService.GetContentCulture(part.MasterContentItem ?? part.ContentItem);
                    availableCultures.Remove(masterCulture);
                }

                var model = new EditLocalizationViewModel
                {
                    SiteCulture = _cultureManager.GetSiteCulture(),
                    SelectedCulture = selectedCulture,
                    AvailableCultures = availableCultures,
                    ContentItem = part,
                    MasterContentItem = part.HasTranslationGroup ? part.MasterContentItem : null,
                    ContentLocalizations = new ContentLocalizationsViewModel(part) { Localizations = localizations }
                };

                return ContentShape("Parts_Localization_ContentTranslations_Edit",
                    () => shapeHelper.EditorTemplate(TemplateName: "Parts/Localization.ContentTranslations.Edit", Model: model, Prefix: TemplatePrefix));
            }
            else
            {
                return null;
            }
        }

        protected override DriverResult Editor(LocalizationPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = new EditLocalizationViewModel();

            // GetCulture(part) is checked against null value, because the content culture has to be set only if it's not set already.
            // model.SelectedCulture is checked against null value, because the editor group may not contain LocalizationPart when the content item is saved for the first time.
            if (updater != null && updater.TryUpdateModel(model, TemplatePrefix, null, null) && GetCulture(part) == null && !string.IsNullOrEmpty(model.SelectedCulture))
            {
                _localizationService.SetContentCulture(part, model.SelectedCulture);
            }

            return Editor(part, shapeHelper);
        }                                                               

        private static string GetCulture(LocalizationPart part)
        {
            return part.Culture != null ? part.Culture.Culture : null;
        }

        private IEnumerable<LocalizationPart> GetDisplayLocalizations(LocalizationPart part, VersionOptions versionOptions)
        {
            return _localizationService.GetLocalizations(part.ContentItem, versionOptions)
                .Select(c =>
                {
                    var localized = c.ContentItem.As<LocalizationPart>();
                    if (localized.Culture == null)
                        localized.Culture = _cultureManager.GetCultureByName(_cultureManager.GetSiteCulture());
                    return c;
                }).ToList();
        }

        private IEnumerable<LocalizationPart> GetEditorLocalizations(LocalizationPart part)
        {
            return _localizationService.GetLocalizations(part.ContentItem, VersionOptions.Latest)
                .Select(c =>
                {
                    var localized = c.ContentItem.As<LocalizationPart>();
                    if (localized.Culture == null)
                        localized.Culture = _cultureManager.GetCultureByName(_cultureManager.GetSiteCulture());
                    return c;
                }).ToList();
        }

        protected override void Importing(LocalizationPart part, ContentManagement.Handlers.ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "MasterContentItem", masterContentItem =>
            {
                var contentItem = context.GetItemFromSession(masterContentItem);
                if (contentItem != null)
                {
                    part.MasterContentItem = contentItem;
                }
            });

            context.ImportAttribute(part.PartDefinition.Name, "Culture", culture =>
            {
                var targetCulture = _cultureManager.GetCultureByName(culture);
                // Add Culture.
                if (targetCulture == null && _cultureManager.IsValidCulture(culture))
                {
                    _cultureManager.AddCulture(culture);
                    targetCulture = _cultureManager.GetCultureByName(culture);
                }
                part.Culture = targetCulture;
            });
        }

        protected override void Exporting(LocalizationPart part, ContentManagement.Handlers.ExportContentContext context)
        {
            if (part.MasterContentItem != null)
            {
                var masterContentItemIdentity = _contentManager.GetItemMetadata(part.MasterContentItem).Identity;
                context.Element(part.PartDefinition.Name).SetAttributeValue("MasterContentItem", masterContentItemIdentity.ToString());
            }

            if (part.Culture != null)
            {
                context.Element(part.PartDefinition.Name).SetAttributeValue("Culture", part.Culture.Culture);
            }
        }
    }
}