using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.SocialLinks.Models;
using Orchard.SocialLinks.ViewModels;
using System;
using System.Collections.Generic;

namespace Orchard.SocialLinks.Drivers
{
    public class FollowMePartDriver : ContentPartDriver<FollowMePart>
    {
        protected override DriverResult Display(FollowMePart part, string displayType, dynamic shapeHelper)
        {
            var links = new List<FollowMeLinkModel>();

            Action<string, string> addService = (name, url) =>
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    links.Add(new FollowMeLinkModel { Name = name, Url = url });
                }
            };

            addService("twitter", part.TwitterUrl);
            addService("facebook", part.FacebookUrl);
            addService("googlePlus", part.GoogleUrl);

            addService("linkedin", part.LinkedInUrl);
            addService("xing", part.XingUrl);

            addService("youtube", part.YouTubeUrl);
            addService("github", part.GitHubUrl);

            return ContentShape("Parts_FollowMePart", () =>
                shapeHelper.Parts_FollowMePart(Links: links));
        }

        //GET
        protected override DriverResult Editor(FollowMePart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_FollowMePart_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts.FollowMePart.Edit",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(
            FollowMePart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

       
    }
}