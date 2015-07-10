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
using System.Xml;
using System.Xml.Schema;
using JFDI.Utils.XSDExtractor.Parsers;

namespace JFDI.Utils.XSDExtractor {
  
  /// <summary>
  /// Responsible for generating a <see cref="XmlSchema"/> object based
  /// on the content of a <see cref="Type"/> object.
  /// </summary>
  public class XSDGenerator {
    
    Dictionary<Type, XmlSchemaComplexType> complexMap;
    Type configType;
    XmlSchema schemaDoc;
    XmlSchemaElement rootElement;

    /// <summary>
    /// Creates an instance of the XsdGenerator class
    /// </summary>
    /// <param name="configType">The is the type which should be examined and converted to an Xsd</param>
    public XSDGenerator(Type configType) {
      this.configType = configType;
      this.complexMap = new Dictionary<Type, XmlSchemaComplexType>();
    }

    /// <summary>
    /// Gets the actual schema object that is created by 
    /// parsing the <see cref="configType"/> object
    /// </summary>
    public XmlSchema Schema {
      get {

        if (schemaDoc == null) {
          schemaDoc = new XmlSchema();
          schemaDoc.TargetNamespace = configType.ToString().ToLower().StartsWith("http://") ? configType.ToString() : "http://" + configType.ToString();
          schemaDoc.ElementFormDefault = XmlSchemaForm.Qualified;
        }

        return schemaDoc;
 
      }

    }

    /// <summary>
    /// Complex map property which tells us which complex types 
    /// have already been created
    /// </summary>
    public Dictionary<Type, XmlSchemaComplexType> ComplexMap {
      get { return complexMap; } 
    }

    /// <summary>
    /// Creates the root element for the schema.
    /// </summary>
    protected XmlSchemaElement CreateRootElement(string rootElementName) {

      //  create the actual root element, use the name of the 
      //  config type object to name it
      rootElement = XMLHelper.CreateElement(string.IsNullOrEmpty(rootElementName) ? configType.Name : rootElementName);

      //  this is the type that the root element is made up of
      XmlSchemaComplexType ct = XMLHelper.CreateComplexType(rootElement.Name);
      Schema.Items.Add(ct);

      //  add the all extension to the complex type so that child elements 
      //  may occur in any order
      XMLHelper.CreateSchemaSequenceParticle(ct);

      //  finally assign the type to the root element
      //  and add it to the document
      rootElement.SchemaTypeName = new XmlQualifiedName(XMLHelper.TargetNamespaceAlias + rootElement.Name + "CT");
      Schema.Items.Add(rootElement);

      return rootElement;

    }

    /// <summary>
    /// Generates a new XmlSchema object from the <see cref="configType"/> object
    /// </summary>
    public XmlSchema GenerateXSD(string rootElementName) {

      rootElement = CreateRootElement(rootElementName);
      XmlSchemaComplexType rootCT = (XmlSchemaComplexType)schemaDoc.Items[0];

      //  get all properties from the configuration object
      foreach (PropertyInfo pi in TypeParser.GetProperties<ConfigurationPropertyAttribute>(configType)) {

        TypeParser parser = TypeParserFactory.GetParser(this, pi);
        parser.GenerateSchemaTypeObjects(pi, rootCT);

      }

      return schemaDoc;

    }

  }
  
}