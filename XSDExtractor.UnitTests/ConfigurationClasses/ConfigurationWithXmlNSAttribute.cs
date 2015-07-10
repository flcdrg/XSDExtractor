using System.Configuration;

namespace JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses
{
    public class ConfigurationWithXmlNSAttribute : ConfigurationSection
    {
        [ConfigurationProperty("xmlns", IsRequired = false)]
        public string Xmlns
        {
            get { return (string) base["xmlns"]; }
        }
    }
}