using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Orchard.ImageGallery {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public AdminMenu() {
            T = NullLocalizer.Instance;
        }

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
          builder.AddImageSet("imagegallery")
            .Add(T("Image Galleries"), "7",
                        menu => menu.Add(T("Image Gallery"), "0", item => item.Action("List", "Admin", new {area = "Orchard.ImageGallery"})
                                                                              .Permission(Permissions.ManageImageGallery)));
        }
    }
}