using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Mvc.Configuration
{
    public class RoutesConfiguration : ConfigurationSection
    {
        private static RoutesConfiguration settings
            = ConfigurationManager.GetSection("routes") as RoutesConfiguration;

        public static RoutesConfiguration Settings
        {
            get
            {
                return settings;
            }
        }    

        [ConfigurationProperty("ignore", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(RoutesConfigurationCollection),
         AddItemName = "add",
         ClearItemsName = "clear",
         RemoveItemName = "remove")]
        public RoutesConfigurationCollection IgnoreRoutes
        {
            get
            {
                return (RoutesConfigurationCollection)base["ignore"];
            }
        }
    }


    public class RoutesConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RoutesConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as RoutesConfigurationElement).Name;
        }

        //IEnumerator<RoutesConfigurationElement> IEnumerable<RoutesConfigurationElement>.GetEnumerator()
        //{
        //    return this.Cast<RoutesConfigurationElement>().GetEnumerator();
        //}

        public RoutesConfigurationElement this[int index]
        {
            get { return (RoutesConfigurationElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
    }

    public class RoutesConfigurationElement : ConfigurationElement
    {
        public RoutesConfigurationElement() { }

        public RoutesConfigurationElement(string name, string url)
        {
            Name = name;
            Url = url;
        }

        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
            set { this["url"] = value; }
        }
    }
}
