using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Core.Navigation.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.UI;
using Orchard.UI.Notify;
using Orchard.UI.Navigation;
using Orchard.Utility;
using System;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.Core.Navigation;
using Orchard.Localization.Services;
using Orchard.Localization.Models;
using Orchard.ExContentManagement.Models;
using Orchard.ContentManagement.Aspects;
using Orchard.ExContentManagement.ViewModels;

namespace Orchard.ExContentManagement.Controllers
{
    [ValidateInput(false), Admin]
    public class NavigationController : Controller, IUpdateModel 
    {           
        private readonly IMenuService _menuService;
        private readonly INavigationManager _navigationManager;
        private readonly IMenuManager _menuManager;
        private readonly ICultureManager _cultureManager;

        public NavigationController(
            IOrchardServices orchardServices,
            IMenuService menuService,
            IMenuManager menuManager,
            INavigationManager navigationManager,
            ICultureManager cultureManager)
        {
            _menuService = menuService;
            _menuManager = menuManager;
            _navigationManager = navigationManager;
            _cultureManager = cultureManager;

            Services = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult Index(NavigationAdminViewModel model, int? menuId, string lang) {
            if (!Services.Authorizer.Authorize(Permissions.ManageMenus, T("Not allowed to manage the main menu"))) {
                return new HttpUnauthorizedResult();
            }

            IEnumerable<TitlePart> menus = Services.ContentManager.Query<TitlePart, TitlePartRecord>().ForType("Menu").List()
                .OrderBy(m =>
                {
                    var orderField = m.ContentItem.Parts.SelectMany(p => p.Fields)
                        .FirstOrDefault(f => f.Name == "Order");

                    if (orderField != null)
                        return orderField.Storage.Get<Decimal?>(null);

                    return m.Id;
                }).ToList();

            if (!menus.Any()) {
                return RedirectToAction("Create", "Admin", new {area = "Contents", id = "Menu", returnUrl = Request.RawUrl});
            }

            IContent currentMenu = menuId == null
                ? menus.FirstOrDefault()
                : menus.FirstOrDefault(menu => menu.Id == menuId);

            if (currentMenu == null && menuId != null) { // incorrect menu id passed
                return RedirectToAction("Index");
            }

            if (model == null) {
                model = new NavigationAdminViewModel();
            }

            var defaultCultureName = _cultureManager.GetSiteCulture();
            var defaultCulture = _cultureManager.GetCultureByName(defaultCultureName);
            model.AvailableCultures = _cultureManager.ListCultures();
            model.CurrentCulture = _cultureManager.GetCurrentCulture(Services.WorkContext.HttpContext);

            model.IsMultyLanguageSite = model.AvailableCultures.Count() > 1;

            if (!string.IsNullOrEmpty(lang) && model.AvailableCultures.Contains(lang, StringComparer.OrdinalIgnoreCase))
            {
                model.CurrentCulture = lang;
            }

            var cultureId = _cultureManager.GetCultureByName(model.CurrentCulture)?.Id;

            if (model.MenuItemEntries == null || !model.MenuItemEntries.Any()) {
                var allParts = Services.ContentManager
                        .Query<MenuPart, MenuPartRecord>()
                        .Where(x => x.MenuId == currentMenu.Id)
                        .List();

                var items = allParts
                    .Where(menuPart =>
                    {
                        var title = menuPart.MenuText;
                        var part = menuPart.As<LocalizationPart>();
                        if (part != null)
                        {
                            if (part.Culture == null)
                                part.Culture = defaultCulture;

                            if (part.Culture?.Id == cultureId)
                            {
                                return true;
                            }
                            else if (part.MasterContentItem == null) // other original item 
                            {
                                var translation = allParts.FirstOrDefault(p => 
                                    p.Has<LocalizationPart>() &&
                                    p.As<LocalizationPart>().Record.MasterContentItemId == part.ContentItem.Id &&
                                    p.As<LocalizationPart>().Record.CultureId == cultureId);

                                return translation == null;  // does not exist translation for current language
                            }
                            else
                                return false;
                        } 
                        return true;
                    })
                    .Select(CreateMenuItemEntries)                     
                    .OrderBy(menuPartEntry => menuPartEntry.Position, new FlatPositionComparer())
                    .ToList();

                items
                    .ForEach(entry => {
                        entry.IsOriginal = entry.CultureId != cultureId;
                    });

                model.MenuItemEntries = items.Cast<MenuItemEntry>().ToList();
            }

            model.MenuItemDescriptors = _menuManager.GetMenuItemTypes();
            model.Menus = menus;
            model.CurrentMenu = currentMenu;


            // need action name as this action is referenced from another action
            return View(model);
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPOST(IList<MenuItemEntry> menuItemEntries, int? menuId, string lang) {
            if (!Services.Authorizer.Authorize(Permissions.ManageMenus, T("Couldn't manage the main menu")))
                return new HttpUnauthorizedResult();

            // See http://orchard.codeplex.com/workitem/17116
            if (menuItemEntries != null) {
                foreach (var menuItemEntry in menuItemEntries) {
                    MenuPart menuPart = _menuService.Get(menuItemEntry.MenuItemId);
                    menuPart.MenuPosition = menuItemEntry.Position;
                }
            }

            return RedirectToAction("Index", new { menuId, lang });
        }

        private ExMenuItemEntry CreateMenuItemEntries(MenuPart menuPart)
        {
            var culture = menuPart.Has<LocalizationPart>() ? menuPart.As<LocalizationPart>().Culture : null;

            return new ExMenuItemEntry {
                MenuItemId = menuPart.Id,
                IsMenuItem = menuPart.Is<MenuItemPart>(),
                Text = menuPart.MenuText,
                Position = menuPart.MenuPosition,
                Url = menuPart.Is<MenuItemPart>()
                              ? menuPart.As<MenuItemPart>().Url
                              : _navigationManager.GetUrl(null, Services.ContentManager.GetItemMetadata(menuPart).DisplayRouteValues),
                ContentItem = menuPart.ContentItem,                
                Culture = culture?.Culture,
                CultureId = culture?.Id
            };
        }

        [HttpPost]
        public ActionResult Delete(int id, string returnUrl) {
            if (!Services.Authorizer.Authorize(Permissions.ManageMenus, T("Couldn't manage the main menu")))
                return new HttpUnauthorizedResult();

            MenuPart menuPart = _menuService.Get(id);
            int? menuId = null;

            if (menuPart != null) {
                menuId = menuPart.Menu.Id;

                // get all sub-menu items from the same menu
                var menuItems = _menuService.GetMenuParts(menuPart.Menu.Id)
                    .Where(x => x.MenuPosition.StartsWith(menuPart.MenuPosition + "."))
                    .Select(x => x.As<MenuPart>())
                    .ToList();

                foreach (var menuItem in menuItems.Concat(new[] { menuPart })) {
                    // if the menu item is a concrete content item, don't delete it, just unreference the menu
                    if (!menuPart.ContentItem.TypeDefinition.Settings.ContainsKey("Stereotype") || menuPart.ContentItem.TypeDefinition.Settings["Stereotype"] != "MenuItem") {
                        menuPart.Menu = null;
                    }
                    else {
                        _menuService.Delete(menuItem);
                    }
                }

            }

            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index", new { menuId }));
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        public ActionResult CreateMenuItem(string id, int menuId, string returnUrl) {
            if (!Services.Authorizer.Authorize(Permissions.ManageMenus, T("Couldn't manage the main menu")))
                return new HttpUnauthorizedResult();

            // create a new temporary menu item
            var menuPart = Services.ContentManager.New<MenuPart>(id);

            if (menuPart == null)
                return HttpNotFound();
            
            // load the menu
            var menu = Services.ContentManager.Get(menuId);

            if (menu == null)
                return HttpNotFound();
            
            try {
                // filter the content items for this specific menu
                menuPart.MenuPosition = Position.GetNext(_navigationManager.BuildMenu(menu));
                
                var model = Services.ContentManager.BuildEditor(menuPart);
                
                return View(model);
            }
            catch (Exception exception) {
                Logger.Error(T("Creating menu item failed: {0}", exception.Message).Text);
                Services.Notifier.Error(T("Creating menu item failed: {0}", exception.Message));
                return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
            }
        }

        [HttpPost, ActionName("CreateMenuItem")]
        public ActionResult CreateMenuItemPost(string id, int menuId, string returnUrl) {
            if (!Services.Authorizer.Authorize(Permissions.ManageMenus, T("Couldn't manage the main menu")))
                return new HttpUnauthorizedResult();

            var menuPart = Services.ContentManager.New<MenuPart>(id);

            if (menuPart == null)
                return HttpNotFound();

            // load the menu
            var menu = Services.ContentManager.Get(menuId);

            if (menu == null)
                return HttpNotFound();
            
            var model = Services.ContentManager.UpdateEditor(menuPart, this);

            menuPart.MenuPosition = Position.GetNext(_navigationManager.BuildMenu(menu));
            menuPart.Menu = menu;

            Services.ContentManager.Create(menuPart);

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return View(model);
            }

            Services.Notifier.Information(T("Your {0} has been added.", menuPart.TypeDefinition.DisplayName));

            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }
	}
}