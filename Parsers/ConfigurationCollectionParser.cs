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
  /// an Xsd Complex type.
  /// </summary>
  public class ConfigurationCollectionParser : TypeParser {

    /// <summary>
    /// Creates an instance of the <see cref="ConfigurationCollectionParser"/> class
    /// </summary>
    public ConfigurationCollectionParser(XSDGenerator generator)
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

      XmlSchemaComplexType ct;
      bool typeAlreadyInSchema = false;
      if (generator.ComplexMap.ContainsKey(property.PropertyType))
      {

        //already done the work
        typeAlreadyInSchema = true;
        ct = generator.ComplexMap[property.PropertyType];

      }
      else
      {

        //  got to generate a new one for the collection
        ct = new XmlSchemaComplexType();
        ct.Name = configAttribute.Name + "CT";
        ct.Particle = new XmlSchemaSequence();

        generator.ComplexMap.Add(property.PropertyType, ct);
        generator.Schema.Items.Add(ct);

      }

      XmlSchemaElement element = new XmlSchemaElement();
      element.Name = configAttribute.Name;
      element.MinOccurs = configAttribute.IsRequired ? 1 : 0;
      element.SchemaTypeName = new XmlQualifiedName(XMLHelper.PrependNamespaceAlias(ct.Name));
      XmlSchemaComplexType pct = type as XmlSchemaComplexType;
      ((XmlSchemaGroupBase)pct.Particle).Items.Add(element);

      //  get all properties from the configuration object
      foreach (PropertyInfo pi in GetProperties<ConfigurationPropertyAttribute>(property.PropertyType)) {

        TypeParser parser = TypeParserFactory.GetParser(generator, pi);
        parser.GenerateSchemaTypeObjects(pi, ct);

      }

      //  add the documentation
      AddAnnotation(property, element, configPropertyAtts[0]);

      //  at this point the element has been added to the schema
      //  but we now need to add support for the child elements
      if (!typeAlreadyInSchema) {
        AddCollectionChildren((XmlSchemaGroupBase)ct.Particle, configCollAttribute);
      }

    }

    /// <summary>
    /// Adds the child elements of the collection
    /// </summary>
    protected void AddCollectionChildren(XmlSchemaGroupBase parentParticle, ConfigurationCollectionAttribute configCollAttribute) {
      
      XmlSchemaComplexType ct;
      if (generator.ComplexMap.ContainsKey(configCollAttribute.ItemType)) {

        //already done the work
        ct = generator.ComplexMap[configCollAttribute.ItemType];

      } else {

        //  got to generate a new one for the collection
        ct = new XmlSchemaComplexType();
        ct.Name = configCollAttribute.ItemType.Name + "CT";
        XMLHelper.CreateSchemaSequenceParticle(ct);// = new XmlSchemaAll();

        generator.ComplexMap.Add(configCollAttribute.ItemType, ct);
        generator.Schema.Items.Add(ct);

      }

      //  ok we now have the child element as a complextype object
      //  but we need to add three elements to the parent complex type
      //  which support the add / remove / clear methods of the collection
      //  if we must add a clear method then it has to come first

      XmlSchemaElement element;

      //  the basic map types do not include the clear and remove methods
      if (configCollAttribute.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMap ||
         configCollAttribute.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMapAlternate) {

        //  clear method (which is a simple element)
        element = new XmlSchemaElement();
        element.Name = configCollAttribute.ClearItemsName;
        element.MinOccurs = 0;
        element.MaxOccurs = 1;
        element.SchemaType = new XmlSchemaComplexType();
        parentParticle.Items.Add(element);

        //  remove method
        element = new XmlSchemaElement();
        element.MinOccurs = 0;
        element.MaxOccursString = "unbounded";
        element.Name = configCollAttribute.RemoveItemName;
        XmlSchemaComplexType removeCT = new XmlSchemaComplexType();
        element.SchemaType = removeCT;

        //  get the type contained in the collection and work out
        //  what the key property is 
        Type childElementType = configCollAttribute.ItemType;
        bool found = false;
        foreach (PropertyInfo pi in childElementType.GetProperties())
        {

          if (found)
            break;

          ConfigurationPropertyAttribute[] cpAtts = GetAttributes<ConfigurationPropertyAttribute>(pi);
          foreach (ConfigurationPropertyAttribute att in cpAtts)
          {
            if (att.IsKey)
            {

              // this should return a standardtypeparser object, if it doesnt
              //  then it means that the key of the element was an element 
              //  itself and I don't think that is possible!
              TypeParser parser = TypeParserFactory.GetParser(generator, pi);
              parser.GenerateSchemaTypeObjects(pi, removeCT);
              found = true;
              break;

            }
          }

        }

        parentParticle.Items.Add(element);

      }

      //  add method
      XmlSchemaElement addElement = new XmlSchemaElement();
      addElement.Name = configCollAttribute.AddItemName;
      addElement.MinOccurs = 0;
      addElement.MaxOccursString = "unbounded";
      addElement.SchemaTypeName = new XmlQualifiedName(XMLHelper.PrependNamespaceAlias(ct.Name));
      parentParticle.Items.Add(addElement);

      //  get all properties from the configuration object
      foreach (PropertyInfo pi in GetProperties<ConfigurationPropertyAttribute>(configCollAttribute.ItemType)) {

        TypeParser parser = TypeParserFactory.GetParser(generator, pi);
        parser.GenerateSchemaTypeObjects(pi, ct);

      }

    }

  }

}