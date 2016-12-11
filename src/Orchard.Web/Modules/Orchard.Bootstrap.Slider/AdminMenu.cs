using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.UI.Navigation;
using Orchard.Bootstrap.Slider.Models;
using Orchard.Core.Contents;
using Orchard.Security;

namespace Orchard.Bootstrap.Slider
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public AdminMenu(IAuthorizationService authorizationService, IWorkContextAccessor workContextAccessor)
        {
            _authorizationService = authorizationService;
            _workContextAccessor = workContextAccessor;
        }

        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("slider")
                .Add(T("Sliders"), "1.5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(T("Manage Sliders"), "3",
                     item => item.Action("List", "SliderAdmin", new { area = "Orchard.Bootstrap.Slider" })
                         .Permission(Permissions.EditContent));
        }
    }
}