using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization.Models;
using Orchard.ExContentManagement.Models;
using Orchard.ExContentManagement.ViewModels;
using Orchard.Core.Title.Models;

namespace Orchard.ExContentManagement.Drivers
{    
    public class ParentContentPartDriver : ContentPartDriver<ParentContentPart>
    {                
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;

        public ParentContentPartDriver(
                IContentManager contentManager,
                IOrchardServices orchardServices) 
        {                      
            _contentManager = contentManager;
            _orchardServices = orchardServices;
        }

        protected override string Prefix
        {
            get
            {
                return "ParentContent";
            }
        }

        protected override DriverResult Display(ParentContentPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_ContentManagement_ParentContent_SummaryAdmin",
                 () => shapeHelper.Parts_ContentManagement_ParentContent_SummaryAdmin());
        }

        protected override DriverResult Editor(ParentContentPart part, dynamic shapeHelper)
        {
            var model = new ParentContentPartEditViewModel();
            ContentItem parentContent = null;
            if (part.ContentItem.Id == 0)
            {
                var parentIdString = _orchardServices.WorkContext.HttpContext.Request.QueryString["parentId"];
                if (!string.IsNullOrEmpty(parentIdString))
                {
                    int id = 0;
                    if (int.TryParse(parentIdString, out id))
                        model.ParentId = id;

                    if (model.ParentId != 0)
                    {
                        parentContent = _contentManager.Get(model.ParentId, VersionOptions.Latest);
                        part.ParentContent = parentContent;
                    }
                }
            }
            else if (part.ParentContent != null)
            {
                parentContent = part.ParentContent.ContentItem;            
            }
            if (parentContent != null)
            {
                model.ParentContent = parentContent;
                model.ContentTypeName = model.ParentContent.TypeDefinition.DisplayName;
                if (model.ParentContent.Has<TitlePart>())
                {
                    model.ParentContentTitle = model.ParentContent.As<TitlePart>().Title;
                }
                else
                {
                    model.ParentContentTitle = string.Format("{0} {1}",
                        model.ContentTypeName, model.ParentId);
                }
            }

            return ContentShape("Parts_ContentManagement_ParentContent_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts/ContentManagement.ParentContent.Edit", Model: model, Prefix: Prefix));
        }

        protected override DriverResult Editor(ParentContentPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = new ParentContentPartEditViewModel();
            updater.TryUpdateModel(model, Prefix, null, null);

            if (model.ParentId != 0)
            {
                var parentContent = _contentManager.Get(model.ParentId, VersionOptions.AllVersions);
                part.ParentContent = parentContent;
            }

            return Editor(part, shapeHelper);
        }
    }
}