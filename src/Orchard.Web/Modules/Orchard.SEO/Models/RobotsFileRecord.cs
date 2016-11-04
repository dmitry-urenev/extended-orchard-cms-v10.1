using Orchard.Data.Conventions;
namespace Orchard.SEO.Models {
	public class RobotsFileRecord {
		public virtual int Id { get; set; }

        [StringLengthMax]
		public virtual string FileContent { get; set; }
	}
}