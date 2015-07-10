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
using System.Configuration;
using System.Reflection;

namespace JFDI.Utils.XSDExtractor.Parsers {
  
  /// <summary>
  /// Returns a parser object based on the attributes / return type found
  /// </summary>
  public static class TypeParserFactory {

    /// <summary>
    /// Returns a new TypeParser object
    /// </summary>
    public static TypeParser GetParser(XSDGenerator generator, PropertyInfo property) {

      ConfigurationPropertyAttribute[] propertyAtts = TypeParser.GetAttributes<ConfigurationPropertyAttribute>(property);
      ConfigurationCollectionAttribute[] collectionAtts = TypeParser.GetAttributes<ConfigurationCollectionAttribute>(property);

      //  this catches any collections which have their attribute on the property
      //  that declares the use of the collection as opposed to on the collection class 
      if (propertyAtts.Length > 0 && (
          collectionAtts.Length > 0 || property.PropertyType.IsSubclassOf(typeof(ConfigurationElementCollection) ))) {

        if (propertyAtts[0].IsDefaultCollection) {

          return new DefaultConfigurationCollectionParser(generator);

        } else {

          return new ConfigurationCollectionParser(generator);

        }
      
      } else if (propertyAtts.Length > 0) {

        if (property.PropertyType.IsSubclassOf(typeof(ConfigurationElement))) {

          return new ConfigurationElementParser(generator);

        } else {

          return new StandardTypeParser(generator);

        }

      } else {
        
        throw new Exception("Neither ConfigurationPropertyAttribute or ConfigurationCollectionAttribute were found in the type.");

      }

    }

  }

}