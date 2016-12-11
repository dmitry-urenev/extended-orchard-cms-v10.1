using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.SocialLinks.Services
{
    public class DefaultShareServiceFactory : IShareServiceFactory
    {
        private IEnumerable<ISocialShareService> _registeredServices;

        public DefaultShareServiceFactory(IEnumerable<ISocialShareService> services)
        {
            _registeredServices = services;
        }

        public ISocialShareService GetService(string name)
        {
            return _registeredServices.FirstOrDefault(s => 
                string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}