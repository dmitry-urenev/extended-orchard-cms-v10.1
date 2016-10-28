using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
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
            var contentTypeDefinitions = _contentDefinitionManager.ListTypeDefinitions().OrderBy(d => d.Name);
            builder
                .AddImageSet("content")
                .Add(T("Content"), "1",
                    menu => menu
                        .Add(T("Content"), "0", item =>
                        {
                            var adminSearchFeature = _moduleService.GetAvailableFeatures().FirstOrDefault(f => f.Descriptor.Id == "Orchard.Search.Content");
                            if (adminSearchFeature != null && adminSearchFeature.IsEnabled)
                            {
                                item.Action("Index", "Admin", new { area = "Orchard.Search" })
                                    .Permission(Orchard.Core.Contents.Permissions.ViewContent)
                                    .Add(T("Search"), "0.0", subItem => subItem
                                        .LocalNav().Action("Index", "Admin", new { area = "Orchard.Search" }))
                                    .Add(T("Content"), "0.1", subItem => subItem
                                        .LocalNav().Action("List", "Pages", new { area = "Orchard.ExContentManagement" }));
                            }
                            else
                            {
                                item.Action("List", "Pages", new { area = "Orchard.ExContentManagement" })
                                    .Permission(Orchard.Core.Contents.Permissions.ViewContent)
                                    .Add(T("Content"), "0.0", subItem => subItem
                                        .LocalNav().Action("List", "Pages", new { area = "Orchard.ExContentManagement" }));
                            }
                        })
                );

            builder.Add(T("Content"), "1",
                    menu => menu.Add(T("Pages"), "1", item => item.Action("List", "Pages", new { area = "Orchard.ExContentManagement", TypeName = "Page" })
                        .Permission(Orchard.Core.Contents.Permissions.ViewContent)
                        ));

          

            //var contentTypes = contentTypeDefinitions.Where(ctd => ctd.Settings.GetModel<ContentTypeSettings>().Creatable).OrderBy(ctd => ctd.DisplayName);
            //if (contentTypes.Any())
            //{
            //    builder.Add(T("New"), "-1", menu =>
            //    {
            //        menu.LinkToFirstChild(false);
            //        foreach (var contentTypeDefinition in contentTypes)
            //        {
            //            var ci = _contentManager.New(contentTypeDefinition.Name);
            //            var cim = _contentManager.GetItemMetadata(ci);
            //            var createRouteValues = cim.CreateRouteValues;
            //            // review: the display name should be a LocalizedString
            //            if (createRouteValues.Any())
            //                menu.Add(T(contentTypeDefinition.DisplayName), "5", item => item.Action(cim.CreateRouteValues["Action"] as string, cim.CreateRouteValues["Controller"] as string, cim.CreateRouteValues)
            //                    // Apply "PublishOwn" permission for the content type
            //                    .Permission(DynamicPermissions.CreateDynamicPermission(DynamicPermissions.PermissionTemplates[Orchard.Core.Contents.Permissions.PublishOwnContent.Name], contentTypeDefinition)));
            //        }
            //    });
            //}

            builder
            .Add(T("Navigation"), "7",
                menu => menu
                    .Add(T("Main Menu"), "0", item => item.Action("Index", "Navigation", new { area = "Orchard.ExContentManagement" })
                    .Permission(Orchard.Core.Navigation.Permissions.ManageMenus)));

            //builder
            // .Add(T("Content"), menu => menu
            //     .Add(T("Sitemap Content"), "2", item => item.Action("Index", "SiteMap", new { area = "Orchard.ExContentManagement" })
            //        .Permission(StandardPermissions.SiteOwner)));
        }
    }
}