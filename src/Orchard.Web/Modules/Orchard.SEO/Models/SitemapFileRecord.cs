using Orchard.Data.Conventions;
namespace Orchard.SEO.Models {
	public class SitemapFileRecord {
		public virtual int Id { get; set; }
        
        [StringLengthMax]
		public virtual string FileContent { get; set; }
	}
}