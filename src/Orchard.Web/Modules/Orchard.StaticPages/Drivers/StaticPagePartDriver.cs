using System;
using System.Linq;
using System.Collections.Generic;

using Orchard.StaticPages.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.StaticPages.ViewModels;
using Orchard.StaticPages.Routing;
using Orchard.StaticPages.Services;

namespace Orchard.StaticPages.Drivers
{
    
    public class StaticPagePartDriver : ContentPartDriver<StaticPagePart>
    {
        protected override string Prefix
        {
            get { return "StaticPagePart"; }
        }

        private readonly IStaticPageService _staticPageService;

        public StaticPagePartDriver(IStaticPageService staticPageService)
        {
            _staticPageService = staticPageService;
        }

        //protected override DriverResult Display(StaticPagePart part, string displayType, dynamic shapeHelper)
        //{
        //    return ContentShape("Parts_StaticPage_SummaryAdmin",
        //            () => shapeHelper.Parts_StaticPage_SummaryAdmin());
        //}

        protected override DriverResult Editor(StaticPagePart pagePart, dynamic shapeHelper)
        {
            StaticPagePartEditViewModel model = new StaticPagePartEditViewModel()
            {
                IsAction = pagePart.IsAction                
            };
            if (!model.IsAction)
            {
                model.StaticPath = pagePart.StaticPageUrl;
            }
            else if (pagePart.Route != null)
            {
                model.Area = pagePart.Route.Action.Area;
                model.Controller = pagePart.Route.Action.Controller;
                model.Action = pagePart.Route.Action.Action;

                List<RouteValue> rv = new List<RouteValue>();
                var rvDictionary = RouteUtils.ParseRouteValues(pagePart.Route.RouteValues);
                foreach (var i in rvDictionary)
                {
                    rv.Add(new RouteValue() { Name = i.Key, Value = i.Value });
                }
                model.RouteValues = rv;
            }

            return ContentShape("Parts_StaticPage_Edit",
                    () => shapeHelper.EditorTemplate(TemplateName: "Parts.StaticPage.Edit", Model: model, Prefix: Prefix));
        }

        protected override DriverResult Editor(StaticPagePart pagePart, IUpdateModel updater, dynamic shapeHelper)
        {
            StaticPagePartEditViewModel model = new StaticPagePartEditViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, null))
            {
                if (!model.IsAction)
                {
                    _staticPageService.UpdateRoute(pagePart, model.StaticPath);
                }
                else
                {
                    var routeValues = model.RouteValues.ToDictionary(r => r.Name, r => r.Value);
                    routeValues.Add("area", model.Area);
                    routeValues.Add("controller", model.Controller);
                    routeValues.Add("action", model.Action);

                    _staticPageService.UpdateRoute(pagePart, routeValues);
                }
            }

            return Editor(pagePart, shapeHelper);
        }
    }
}