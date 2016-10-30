using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents;
using Orchard.Core.Contents.Settings;
using Orchard.Core.Navigation;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Modules.Services;
using Orchard.Security;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Orchard.ExContentManagement
{
    [OrchardSuppressDependency("Orchard.Core.Contents.AdminMenu")]
    [OrchardSuppressDependency("Orchard.Core.Navigation.AdminMenu")]
    public class AdminMenu : INavigationProvider
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly IModuleService _moduleService;

        public AdminMenu(IContentDefinitionManager contentDefinitionManager, IContentManager contentManager, IModuleService moduleService)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _moduleService = moduleService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            var contentTypeDefinitions = _contentDefinitionManager
                .ListTypeDefinitions().OrderBy(d => d.Name);
            var contentTypes = contentTypeDefinitions
                .Where(ctd => ctd.Settings.GetModel<ContentTypeSettings>().Creatable)
                .OrderBy(ctd => ctd.DisplayName);
                               

            builder
                .AddImageSet("content")
                .AddImageSet("excontentmanagement")
                .Add(T("Content"), "1.4", menu => {
                    menu.LinkToFirstChild(false);
                    foreach (var contentTypeDefinition in contentTypes)
                    {
                        if (string.Compare(
                            contentTypeDefinition.Settings["ContentTypeSettings.Creatable"],
                            "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            ContentTypeDefinition definition = contentTypeDefinition;
                            menu.Add(T(contentTypeDefinition.DisplayName), "5", item =>
                                item.Action("List", "Pages",
                                new RouteValueDictionary {
                                        {"area", "Orchard.ExContentManagement"},
                                        {"TypeName", definition.Name}
                                })
                                .Permission(Core.Contents.DynamicPermissions.CreateDynamicPermission(
                                    Core.Contents.DynamicPermissions.PermissionTemplates["PublishOwnContent"],
                                    definition)));
                        }
                    }
                    var adminSearchFeature = _moduleService.GetAvailableFeatures().FirstOrDefault(f => f.Descriptor.Id == "Orchard.Search.Content");
                    if (adminSearchFeature != null && adminSearchFeature.IsEnabled)
                    {
                        menu.Add(T("Search"), "10", item => item
                                .Action("Index", "Admin", new { area = "Orchard.Search" }));
                    }
                });
               
            builder
            .Add(T("Navigation"), "7",
                menu => menu
                    .Add(T("Main Menu"), "0", item => item.Action("Index", "Navigation", new { area = "Orchard.ExContentManagement" })
                    .Permission(Orchard.Core.Navigation.Permissions.ManageMenus)));
        }
    }
}