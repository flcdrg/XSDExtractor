using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MultiFileWatcher.Configuration {
  
  /// <summary>
  /// Provides support for the MultiFileWatcher configuration
  /// </summary>
  public class MultiWatcherConfigurationSection  : ConfigurationSection {

    /// <summary>
    /// Xml Namespace attribute. Added so that the parser doesnt complain
    /// </summary>
    [ConfigurationProperty("xmlns", IsRequired = false)]
    public string Xmlns {
      get { return (string)base["xmlns"]; }
    }

    /// <summary>
    /// Collection of files that will be monitored by the application
    /// </summary>
    [ConfigurationProperty("files", IsDefaultCollection=true, IsKey=false, IsRequired=true)]
    public FilesCollection Files {
      get { return (FilesCollection)base["files"]; }
    }

  }

}
