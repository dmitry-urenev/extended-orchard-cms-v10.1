using Orchard.UI.Resources;

namespace Orchard.SocialLinks
{
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineStyle("SocialLinks").SetUrl("socialLinks.css");
        }
    }
}
