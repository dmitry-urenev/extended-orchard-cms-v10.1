using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Orchard.SEO
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ConfigureRobotsTextFile = new Permission { Description = "Configure Robots.txt", Name = "ConfigureRobotsTextFile" };
        public static readonly Permission ConfigureSitemapFile = new Permission { Description = "Configure Sitemap.xml", Name = "ConfigurSitemapFile" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] { ConfigureRobotsTextFile, ConfigureSitemapFile };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] { new PermissionStereotype { Name = "Administrator", Permissions = new[] { 
                ConfigureRobotsTextFile, ConfigureSitemapFile
            } } };
        }
    }
}