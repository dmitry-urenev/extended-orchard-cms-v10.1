using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.SocialLinks.Models;
using Orchard.SocialLinks.Services;
using Orchard.SocialLinks.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Orchard.Core.Contents.PageContext;

namespace Orchard.SocialLinks.Drivers
{
    public class SocialSharePartDriver : ContentPartDriver<SocialSharePart>
    {
        private IShareServiceFactory _shareServiceFactory;
        private ISocialContextAccessor _socialContextAccessor;


        public SocialSharePartDriver(
            IShareServiceFactory shareServiceFactory,
            ISocialContextAccessor socialContextAccessor)
        {
            _shareServiceFactory = shareServiceFactory;
            _socialContextAccessor = socialContextAccessor;
        }

        public IOrchardServices Services { get; private set; }

        protected override DriverResult Display(SocialSharePart part, string displayType, dynamic shapeHelper)
        {
            List<ISocialShareService> services = new List<ISocialShareService>();

            if (part.EnableTwitter)
            {
                services.Add(_shareServiceFactory.GetService("twitter"));
            }
            if (part.EnableFacebook)
            {
                services.Add(_shareServiceFactory.GetService("facebook"));
            }
            if (part.EnableGoogle)
            {
                services.Add(_shareServiceFactory.GetService("googlePlus"));
            }
            if (part.EnableLinkedIn)
            {
                services.Add(_shareServiceFactory.GetService("linkedIn"));
            }
            if (part.EnableXing)
            {
                services.Add(_shareServiceFactory.GetService("xing"));
            }
            services = services.Where(s => s != null).ToList();

            var context = _socialContextAccessor.Current();

            if (context != null)
            {
                var buttons = services.Select(s => s.Apply(context))
                    .Where(e => e != null)
                    .ToList();

                return ContentShape("Parts_SocialSharePart", () =>
                    shapeHelper.Parts_SocialSharePart(Buttons: buttons));
            }

            return null;
        }

        //GET
        protected override DriverResult Editor(SocialSharePart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_SocialSharePart_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts.SocialSharePart.Edit",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(
            SocialSharePart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}