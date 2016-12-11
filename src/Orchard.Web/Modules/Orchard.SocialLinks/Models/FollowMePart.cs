using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;


namespace Orchard.SocialLinks.Models
{
    public class FollowMePartRecord : ContentPartRecord
    {
        public virtual string TwitterUrl { get; set; }
        public virtual string FacebookUrl { get; set; }
        public virtual string GoogleUrl { get; set; }
        public virtual string GitHubUrl { get; set; }
        public virtual string LinkedInUrl { get; set; }
        public virtual string YouTubeUrl { get; set; }
        public virtual string XingUrl { get; set; }
    }

    public class FollowMePart : ContentPart<FollowMePartRecord>
    {
        public string TwitterUrl
        {
            get { return Record.TwitterUrl; }
            set { Record.TwitterUrl = value; }
        }

        public string FacebookUrl
        {
            get { return Record.FacebookUrl; }
            set { Record.FacebookUrl = value; }
        }

        public string GoogleUrl
        {
            get { return Record.GoogleUrl; }
            set { Record.GoogleUrl = value; }
        }

        public string GitHubUrl
        {
            get { return Record.GitHubUrl; }
            set { Record.GitHubUrl = value; }
        }

        public string LinkedInUrl
        {
            get { return Record.LinkedInUrl; }
            set { Record.LinkedInUrl = value; }
        }

        public string XingUrl
        {
            get { return Record.XingUrl; }
            set { Record.XingUrl = value; }
        }

        public string YouTubeUrl
        {
            get { return Record.YouTubeUrl; }
            set { Record.YouTubeUrl = value; }
        }
    }

}