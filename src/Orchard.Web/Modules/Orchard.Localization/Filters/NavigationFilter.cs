using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization.Services;
using Orchard.UI.Navigation;
using System.Collections.Generic;

namespace Orchard.Localization.Filters
{
    public class NavigationFilter : INavigationFilter
    {
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public NavigationFilter(ICultureManager cultureManager, IWorkContextAccessor workContextAccessor)
        {
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
        }

        #region INavigationFilter Members

        public IEnumerable<MenuItem> Filter(IEnumerable<MenuItem> menuItems)
        {
            string currentCulture = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);
            string defaultCulture = _cultureManager.GetSiteCulture();

            foreach (MenuItem menuItem in menuItems)
            {
                ILocalizableAspect localizationPart = menuItem.Content.As<ILocalizableAspect>();

                if (localizationPart == null
                    || (localizationPart.Culture == null && currentCulture == defaultCulture)
                    || localizationPart.Culture == currentCulture)
                    yield return menuItem;
            }
        }

        #endregion
    }
}