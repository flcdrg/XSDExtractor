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
using System.Xml.Schema;
using System.Xml;
using System.ComponentModel;

namespace JFDI.Utils.XSDExtractor.Parsers {
  
  /// <summary>
  /// Base class for all type parsers
  /// </summary>
  public abstract class TypeParser {

    protected XSDGenerator generator;

    /// <summary>
    /// Creates an instance of the TypeParser object. CTOR is protected
    /// because only inheriting classes should be able to use this constructor
    /// </summary>
    protected TypeParser(XSDGenerator generator) {
      this.generator = generator;
    }

    /// <summary>
    /// Returns all properties found in the type which have either a <see cref="ConfigurationPropertyAttribute"/>
    /// or a <see cref="ConfigurationCollectionAttribute"/> attribute.
    /// </summary>
    public static PropertyInfo[] GetProperties<AttributeType>(Type t) where AttributeType : Attribute {

      List<PropertyInfo> props = new List<PropertyInfo>();

      //  get all properties that have the appropriate atrribute on them
      foreach (PropertyInfo pi in t.GetProperties()) {

        object[] atts = pi.GetCustomAttributes(typeof(AttributeType), true);
        if (atts.Length > 0) {
          props.Add(pi);
        }

      }

      return props.ToArray();

    }

    /// <summary>
    /// Returns all attributes found in the propertyinfo object which match the generic type property
    /// </summary>
    public static AttributeType[] GetAttributes<AttributeType>(Type type) where AttributeType : Attribute {

      object[] atts = type.GetCustomAttributes(typeof(AttributeType), true);
      AttributeType[] retval = new AttributeType[atts.Length];

      //  get all properties that have the appropriate atrribute on them
      int i = 0;
      foreach (object objAttr in atts) {
        retval[i++] = (AttributeType)objAttr;
      }

      return retval;

    }

    /// <summary>
    /// Returns all attributes found in the propertyinfo object which match the generic type property
    /// </summary>
    public static AttributeType[] GetAttributes<AttributeType>(PropertyInfo pi) where AttributeType : Attribute {

      object[] atts = pi.GetCustomAttributes(typeof(AttributeType), true);
      AttributeType[] retval = new AttributeType[atts.Length];

      //  get all properties that have the appropriate atrribute on them
      int i=0;
      foreach (object objAttr in atts) {
        retval[i++] = (AttributeType)objAttr;
      }

      return retval;

    }

    /// <summary>
    /// Abstract method which, when overriden in super classes, allows the
    /// class to actually parse the <paramref name="property" /> and decide which 
    /// schema objects should be created
    /// </summary>
    public abstract void GenerateSchemaTypeObjects(PropertyInfo property, XmlSchemaType type);

    /// <summary>
    /// Provides standard documentation for a type in the form of XmlSchemaDocumentation objects
    /// </summary>
    protected void AddAnnotation(PropertyInfo property, XmlSchemaAnnotated annotatedType, ConfigurationPropertyAttribute configProperty) {

      annotatedType.Annotation = new XmlSchemaAnnotation();

      //  human documentation
      DescriptionAttribute[] descriptionAtts = GetAttributes<DescriptionAttribute>(property);
      
      //  standard info
      string standardDesc = configProperty.IsRequired ? "Required" : "Optional";
      standardDesc += " " + property.PropertyType.FullName;
      standardDesc += " " + (configProperty.DefaultValue.ToString() == "System.Object" ? "" : "[" + configProperty.DefaultValue.ToString() + "]");

      XmlSchemaDocumentation documentation = new XmlSchemaDocumentation();
      if (descriptionAtts.Length > 0) {
        documentation.Markup = TextToNodeArray(descriptionAtts[0].Description + " " + standardDesc);
      } else {
        documentation.Markup = TextToNodeArray(standardDesc);
      }

      //  machine documentation
      XmlSchemaAppInfo appInfo = new XmlSchemaAppInfo();
      appInfo.Markup = TextToNodeArray(string.Format("{0}{1}", property.DeclaringType.FullName, property.Name));

      //  add the documentation to the object
      annotatedType.Annotation.Items.Add(documentation);
      annotatedType.Annotation.Items.Add(appInfo);

    }

    private XmlNode[] TextToNodeArray(string text) {
      XmlDocument doc = new XmlDocument();
      return new XmlNode[1] { doc.CreateTextNode(text) };
    }

  }

}