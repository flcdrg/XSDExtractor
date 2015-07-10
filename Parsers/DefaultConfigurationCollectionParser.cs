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

using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System;

namespace JFDI.Utils.XSDExtractor.Parsers {

  /// <summary>
  /// Responsible for converting a <see cref="ConfigurationElementCollection "/> into 
  /// an Xsd Complex type when the collection element is actually the default collection 
  /// for its parent type.
  /// </summary>
  public class DefaultConfigurationCollectionParser : ConfigurationCollectionParser {

    /// <summary>
    /// Creates an instance of the <see cref="ConfigurationElementCollection"/> class
    /// </summary>
    public DefaultConfigurationCollectionParser(XSDGenerator generator)
      : base(generator) { }

    /// <summary>
    /// Convert the property into a schema object
    /// </summary>
    public override void GenerateSchemaTypeObjects(PropertyInfo property, XmlSchemaType type) {

      ConfigurationPropertyAttribute[] configPropertyAtts = GetAttributes<ConfigurationPropertyAttribute>(property);
      if (configPropertyAtts.Length == 0)
        return;

      ConfigurationCollectionAttribute[] configCollPropertyAtts = GetAttributes<ConfigurationCollectionAttribute>(property);
      if (configCollPropertyAtts.Length == 0)
        configCollPropertyAtts = GetAttributes<ConfigurationCollectionAttribute>(property.PropertyType);
      if (configCollPropertyAtts.Length == 0)
        return;

      ConfigurationPropertyAttribute configAttribute = configPropertyAtts[0];
      ConfigurationCollectionAttribute configCollAttribute = configCollPropertyAtts[0];

      //  we are actually going to add the collection to the parent type by creating
      //  a new group type that consists of a sequence of all the elements that we
      //  expect in the collection
      XmlSchemaGroup groupParticle = XMLHelper.CreateGroupType(property.DeclaringType.FullName + "." + property.PropertyType.Name);
      groupParticle.Particle = new XmlSchemaSequence();

      //  add support for the child elements
      AddCollectionChildren(groupParticle.Particle, configCollAttribute);

      //  now add the group to the schema and the parent CT
      generator.Schema.Items.Add(groupParticle);
      
      XmlSchemaComplexType parentCT = type as XmlSchemaComplexType;
      XmlSchemaGroupRef groupRef = new XmlSchemaGroupRef();
      groupRef.RefName = new XmlQualifiedName(XMLHelper.PrependNamespaceAlias(groupParticle.Name));
      ((XmlSchemaGroupBase)parentCT.Particle).Items.Add(groupRef);

      //  add the documentation
      AddAnnotation(property, groupRef, configPropertyAtts[0]);

    }

  }

}