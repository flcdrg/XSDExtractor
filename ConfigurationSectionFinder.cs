#region License
/*
JFDI the .Net Job Framework (http://jfdi.sourceforge.net)
Copyright (C) 2006  Steven Ward (steve.ward.uk@gmail.com)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
*/
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace JFDI.Utils.XSDExtractor {
  
  /// <summary>
  /// Responsible for finding all the types in the assembly which 
  /// inherit from the ConfigurationSection class
  /// </summary>
  public class ConfigurationSectionFinder {
    
    string assemblyLocation;
    
    public ConfigurationSectionFinder(string assemblyLocation) {
      this.assemblyLocation = assemblyLocation;  
    }

    public Type[] GetConfigSectionTypes(string className) {

      List<Type> tmpArray = new List<Type>();
      Assembly ass = Assembly.LoadFrom(assemblyLocation);
      Type[] types = ass.GetTypes();
      foreach (Type t in types) {
        if ((!string.IsNullOrEmpty(className) && t.FullName == className) || string.IsNullOrEmpty(className)) {
          if (t.IsSubclassOf(typeof(ConfigurationSection))) {
            tmpArray.Add(t);
          }
        }
      }

      return tmpArray.ToArray();
      
    }
    
  }
  
}