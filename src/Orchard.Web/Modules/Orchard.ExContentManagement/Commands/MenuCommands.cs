using Orchard.Commands;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentPermissions.Models;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Localization.Models;
using Orchard.Localization.Records;
using Orchard.Localization.Services;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ExContentManagement.Commands
{
    public class MenuCommands : DefaultOrchardCommandHandler
    {
        private readonly IContentManager _contentManager;
        private readonly IMenuService _menuService;
        private readonly ISiteService _siteService;
        private readonly IMembershipService _membershipService;
        private readonly ICultureManager _cultureManager;
        private readonly IRoleService _roleService;

        public MenuCommands(
            IContentManager contentManager,
            IMenuService menuService,
            ISiteService siteService,
            IMembershipService membershipService,
            ICultureManager cultureManager,
            IRoleService roleService)
        {
            _contentManager = contentManager;
            _menuService = menuService;
            _siteService = siteService;
            _membershipService = membershipService;
            _cultureManager = cultureManager;
            _roleService = roleService;
        }

        [OrchardSwitch]
        public string MenuPosition { get; set; }

        [OrchardSwitch]
        public string Owner { get; set; }

        [OrchardSwitch]
        public string MenuText { get; set; }

        [OrchardSwitch]
        public string Url { get; set; }

        [OrchardSwitch]
        public string MenuName { get; set; }

        [OrchardSwitch]
        public string Master { get; set; }

        [OrchardSwitch]
        public string Culture { get; set; }

        [OrchardSwitch]
        public string Roles { get; set; }

        [CommandName("menuitem create-ex")]
        [CommandHelp("menuitem create-ex /MenuPosition:<position> /MenuText:<text> /Url:<url> /MenuName:<name> [/Owner:<username>] /Master:<item name> /Culture:<code> /Roles:<role1,role2,..> \r\n\t" + "Creates a new menu item")]
        [OrchardSwitches("MenuPosition,MenuText,Url,MenuName,Owner,Master,Culture,Roles")]
        public void Create()
        {
            // flushes before doing a query in case a previous command created the menu

            var menu = _menuService.GetMenu(MenuName);

            if (menu == null)
            {
                Context.Output.WriteLine(T("Menu not found.").Text);
                return;
            }

            var menuItem = _contentManager.Create("MenuItem");
            menuItem.As<MenuPart>().MenuPosition = MenuPosition;
            menuItem.As<MenuPart>().MenuText = T(MenuText).ToString();
            menuItem.As<MenuPart>().Menu = menu.ContentItem;
            menuItem.As<MenuItemPart>().Url = Url;

            if (menuItem.Is<LocalizationPart>())
            {
                var defaultCulture = _cultureManager.GetCultureByName(_cultureManager.GetSiteCulture());
                CultureRecord culture = null;
                if (!string.IsNullOrEmpty(Culture))
                {
                    culture = _cultureManager.GetCultureByName(Culture);
                }
                culture = culture ?? defaultCulture;

                if (!string.IsNullOrEmpty(Master))
                {
                    // Get "original" master, that was created with the current site culture.
                    var masterItem = _menuService.GetMenuParts(menu.Id).FirstOrDefault(part =>
                        string.Equals(part.MenuText, Master, StringComparison.OrdinalIgnoreCase) &&
                            part.Is<LocalizationPart>() &&
                            (part.As<LocalizationPart>().Culture == null ||
                                part.As<LocalizationPart>().Culture.Culture == defaultCulture.Culture)
                        );
                    menuItem.As<LocalizationPart>().MasterContentItem = masterItem != null ? masterItem.ContentItem : null;
                }
                menuItem.As<LocalizationPart>().Culture = culture;
            }
            if (menuItem.Is<ContentPermissionsPart>() && !string.IsNullOrEmpty(Roles))
            {
                var definedRoles = Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var roles = _roleService.GetRoles()
                    .Where(r => definedRoles.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
                    .Select(r => r.Name)
                    .ToArray();

                if (roles.Length > 0)
                {
                    menuItem.As<ContentPermissionsPart>().Enabled = true;
                    menuItem.As<ContentPermissionsPart>().ViewContent = string.Join(",", roles);
                }
            }

            if (String.IsNullOrEmpty(Owner))
            {
                Owner = _siteService.GetSiteSettings().SuperUser;
            }
            var owner = _membershipService.GetUser(Owner);

            if (owner == null)
            {
                Context.Output.WriteLine(T("Invalid username: {0}", Owner));
                return;
            }

            menuItem.As<ICommonPart>().Owner = owner;

            Context.Output.WriteLine(T("Menu item created successfully.").Text);
        }

        //[CommandName("menuitem create-ex")]
        //public void CreateButch()
        //{

        //}

    }
}