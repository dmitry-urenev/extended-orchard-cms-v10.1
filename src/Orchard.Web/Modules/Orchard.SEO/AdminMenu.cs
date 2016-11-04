using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Orchard.SEO
{
    [OrchardFeature("Orchard.Robots")]
    public class AdminMenuRobots : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Robots.txt"), "50",
                menu => menu.Add(T("Robots.txt"), "20", item => item.Action("Edit", "Robots", new { area = "Orchard.SEO" })
                    .Permission(Permissions.ConfigureRobotsTextFile)));
        }
    }

    [OrchardFeature("Orchard.Sitemap")]
    public class AdminMenuSitemap : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Sitemap.xml"), "50",
                menu => menu.Add(T("Sitemap.xml"), "21", item => item.Action("Edit", "Sitemap", new { area = "Orchard.SEO" })
                    .Permission(Permissions.ConfigureSitemapFile)));
        }
    }
}