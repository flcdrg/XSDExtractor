using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MultiFileWatcher.Configuration {
  
  /// <summary>
  /// Represents a collection of file elements
  /// </summary>
  [ConfigurationCollection(typeof(FileElement), AddItemName="add", CollectionType=ConfigurationElementCollectionType.BasicMap)]
  public class FilesCollection : ConfigurationElementCollection {

    protected override ConfigurationElement CreateNewElement() {
      return new FileElement();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      FileElement fe = element as FileElement;
      if (fe == null)
        throw new Exception(string.Format("Element was not of type {0}", typeof(FileElement).FullName));
      return fe.Path;
    }
  }

}
