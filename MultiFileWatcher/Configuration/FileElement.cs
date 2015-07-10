using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;

namespace MultiFileWatcher.Configuration {
  
  /// <summary>
  /// Represents a single file in the configuration
  /// </summary>
  public class FileElement : ConfigurationElement {

    /// <summary>
    /// Path to the file
    /// </summary>
    [ConfigurationProperty("path", IsKey=true, IsRequired=true)]
    public string Path {
      get { return (string)base["path"]; }
    }

    /// <summary>
    /// Notify filters 
    /// </summary>
    [ConfigurationProperty("notifyFilter", IsKey=false, IsRequired=false, DefaultValue=NotifyFilters.FileName)]
    public NotifyFilters NotifyFilter {
      get { return (NotifyFilters)base["notifyFilter"]; }
    }

  }

}