using Orchard.UI.Resources;

namespace Orchard.Bootstrap
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("BootstrapAdmin").SetUrl("bootstrap-admin.css").SetVersion("3.2.0");

            //v3.3.4 

            manifest.DefineScript("Bootstrap-v3.3.4").SetUrl("bootstrap-v3.3.4.min.js", "bootstrap-v3.3.4.js").SetVersion("3.3.4")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap-v3.3.4.min.js")
                .SetDependencies("jQuery");

            manifest.DefineStyle("Bootstrap-v3.3.4").SetUrl("bootstrap-v3.3.4.min.css", "bootstrap-v3.3.4.css").SetVersion("3.3.4")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap-v3.3.4.min.css");

            manifest.DefineStyle("BootstrapTheme-v3.3.4").SetUrl("bootstrap-theme-v3.3.4.min.css", "bootstrap-theme-v3.3.4.css").SetVersion("3.3.4")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap-theme.min.css");
        }
    }
}
