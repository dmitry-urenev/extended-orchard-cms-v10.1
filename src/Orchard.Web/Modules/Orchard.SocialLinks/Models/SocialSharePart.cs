using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;


namespace Orchard.SocialLinks.Models
{
    public class SocialSharePart : ContentPart
    {
        public bool EnableTwitter
        {
            get { return this.Retrieve(p => p.EnableTwitter); }
            set { this.Store(p => p.EnableTwitter, value); }
        }

        public bool EnableFacebook
        {
            get { return this.Retrieve(p => p.EnableFacebook); }
            set { this.Store(p => p.EnableFacebook, value); }
        }

        public bool EnableGoogle
        {
            get { return this.Retrieve(p => p.EnableGoogle); }
            set { this.Store(p => p.EnableGoogle, value); }
        }

        public bool EnableXing
        {
            get { return this.Retrieve(p => p.EnableXing); }
            set { this.Store(p => p.EnableXing, value); }
        }

        public bool EnableLinkedIn
        {
            get { return this.Retrieve(p => p.EnableLinkedIn); }
            set { this.Store(p => p.EnableLinkedIn, value); }
        }
    }

}