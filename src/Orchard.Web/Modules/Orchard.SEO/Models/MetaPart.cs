using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Orchard.SEO.Models
{
    public class MetaPart : ContentPart<MetaRecord>
    {
        public string Title
        {
            get { return Record.Title; }
            set { Record.Title = value; }
        }

        public string Description
        {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public string Keywords
        {
            get { return Record.Keywords; }
            set { Record.Keywords = value; }
        }

        public string Robots
        {
            get { return Record.Robots; }
            set { Record.Robots = value; }
        }
    }

    public class MetaRecord : ContentPartRecord
    {
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string Keywords { get; set; }
        public virtual string Robots { get; set; }
    }
}