using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.UI.Resources;
using Orchard.SEO.Models;
using Orchard.Environment.Extensions;

namespace Orchard.SEO.Drivers
{
    [OrchardFeature("Orchard.SEO")]
    public class MetaDriver : ContentPartDriver<MetaPart>
    {
        private readonly IWorkContextAccessor _wca;

        public MetaDriver(IWorkContextAccessor workContextAccessor)
        {
            _wca = workContextAccessor;
        }

        protected override DriverResult Display(MetaPart part, string displayType, dynamic shapeHelper)
        {
            var workContext = _wca.GetContext();
            var resourceManager = workContext.Resolve<IResourceManager>();
            if (!String.IsNullOrWhiteSpace(part.Title))
            {
                resourceManager.SetMeta(new MetaEntry
                {
                    Name = "title",
                    Content = part.Title
                });
                workContext.Layout.HeaderTitle = part.Title;
            }
            if (!String.IsNullOrWhiteSpace(part.Description))
            {
                resourceManager.SetMeta(new MetaEntry
                {
                    Name = "description",
                    Content = part.Description
                });
            }
            if (!String.IsNullOrWhiteSpace(part.Keywords))
            {
                resourceManager.SetMeta(new MetaEntry
                {
                    Name = "keywords",
                    Content = part.Keywords
                });
            }
            //if (!String.IsNullOrWhiteSpace(part.Robots))
            //{
                resourceManager.SetMeta(new MetaEntry
                {
                    Name = "robots",
                    Content = string.IsNullOrEmpty(part.Robots) ? RobotsMetaValues.Default : part.Robots // Default
                });
            //}
            return null;
        }

        //GET
        protected override DriverResult Editor(MetaPart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_Meta_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Meta",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(MetaPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}