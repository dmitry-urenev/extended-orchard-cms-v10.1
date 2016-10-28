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

            manifest.DefineScript("Bootstrap").SetUrl("v3.3.4/bootstrap.min.js", "v3.3.4/bootstrap.js").SetVersion("3.3.4")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js")
                .SetDependencies("jQuery");

            manifest.DefineStyle("Bootstrap").SetUrl("v3.3.4/bootstrap.min.css", "v3.3.4/bootstrap.css").SetVersion("3.3.4")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css");

            manifest.DefineStyle("BootstrapTheme").SetUrl("v3.3.4/bootstrap-theme.min.css", "v3.3.4/bootstrap-theme.css").SetVersion("3.3.4")
                .SetCdn("//maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap-theme.min.css");
        }
    }
}
