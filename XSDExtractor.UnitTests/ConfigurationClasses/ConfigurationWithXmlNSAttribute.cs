using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;

namespace JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses {
  
  public class ConfigurationWithXmlNSAttribute : ConfigurationSection {

    [ConfigurationProperty("xmlns", IsRequired = false)]
    public string Xmlns {
      get { return (string)base["xmlns"]; }
    }

  }

}
