using Orchard.Localization;
using Orchard.UI.Navigation;
using Orchard.Security;
using Orchard.Core.Contents;

namespace Orchard.PageScripts
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Page Scripts"), "2.0", item => item.Action("Index", "Admin", new { area = "Orchard.PageScripts" })
                    .Permission(Permissions.EditContent));
        }
    }
}
