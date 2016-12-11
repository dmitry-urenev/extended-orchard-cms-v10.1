using Orchard.UI.Resources;

namespace Orchard.Bootstrap.Slider
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            //builder.Add().DefineStyle("FlexSlider").SetUrl("flexslider.min.css", "flexslider.css").SetDependencies("Orchard.Bootstrap");

            //builder.Add().DefineScript("FlexSlider").SetUrl("jquery.flexslider.min.js", "jquery.flexslider.js").SetDependencies("jQuery");
        }
    }
}