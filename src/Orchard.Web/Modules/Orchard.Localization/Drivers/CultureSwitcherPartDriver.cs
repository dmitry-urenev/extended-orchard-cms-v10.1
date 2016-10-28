using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using System.Collections.Generic;
using System.Linq;

namespace Orchard.Localization.Drivers
{
    public class CultureSwitcherPartDriver : ContentPartDriver<CultureSwitcherPart> {
        private readonly IOrchardServices _orchardServices;
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;         

        public CultureSwitcherPartDriver(
            IOrchardServices orchardServices, 
            ICultureManager cultureManager, 
            IWorkContextAccessor workContextAccessor) {
            _orchardServices = orchardServices;
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
        }

        protected override DriverResult Display(CultureSwitcherPart part, string displayType, dynamic shapeHelper)
        {
            var cultures = _cultureManager.ListCultures().ToList();
            var storedCultures = part.CultureOrderedList;

            var availableCultures = new List<string>();
            foreach (var c in storedCultures)
            {
                if (cultures.Contains(c))
                    availableCultures.Add(c);
            }
            foreach (var c in cultures)
            {
                if (!availableCultures.Contains(c))
                    availableCultures.Add(c);
            }

            part.AvailableCultures = availableCultures;
            part.UserCulture = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);

            return ContentShape("Parts_Localization_CultureSwitcher", 
                () => shapeHelper.Parts_Localization_CultureSwitcher(
                    AvailableCultures: part.AvailableCultures, 
                    UserCulture: part.UserCulture,
                    DisplayType: part.DisplayType,
                    DisplayMode: part.DisplayMode 
                )
            );
        }


        protected override DriverResult Editor(CultureSwitcherPart part, dynamic shapeHelper)
        {
            var cultures = _cultureManager.ListCultures().ToList();
            var storedCultures = part.CultureOrderedList;

            var availableCultures = new List<string>();
            foreach (var c in storedCultures)
            {
                if (cultures.Contains(c) && !availableCultures.Contains(c))
                    availableCultures.Add(c);
            }
            foreach (var c in cultures)
            {
                if (!availableCultures.Contains(c))
                    availableCultures.Add(c);
            }

            part.AvailableCultures = availableCultures;

            return ContentShape(
                "Parts_Localization_CultureSwitcher_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Localization.CultureSwitcher.Edit",
                    Model: part,
                    Prefix: Prefix)
            );
        }


        protected override DriverResult Editor(CultureSwitcherPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(part, Prefix, null, null))
            {
                part.CultureOrderedList = part.AvailableCultures;
            }  
            return Editor(part, shapeHelper);
        }
    }
}