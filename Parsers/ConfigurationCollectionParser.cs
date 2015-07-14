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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace JFDI.Utils.XSDExtractor.Parsers
{
    /// <summary>
    ///     Responsible for converting a <see cref="ConfigurationElementCollection " /> into
    ///     an Xsd Complex type.
    /// </summary>
    public class ConfigurationCollectionParser : TypeParser
    {
        /// <summary>
        ///     Creates an instance of the <see cref="ConfigurationCollectionParser" /> class
        /// </summary>
        public ConfigurationCollectionParser(XsdGenerator generator)
            : base(generator)
        {
        }

        /// <summary>
        ///     Convert the property into a schema object
        /// </summary>
        public override void GenerateSchemaTypeObjects(PropertyInfo property, XmlSchemaType type, int level)
        {
            Debug.IndentLevel = level;
            Debug.WriteLine("{0} {1} {2}", level, property.Name, type.Name);

            var configPropertyAtts = GetAttributes<ConfigurationPropertyAttribute>(property);
            if (configPropertyAtts.Length == 0)
                return;

            var configCollPropertyAtts = GetAttributes<ConfigurationCollectionAttribute>(property);
            if (configCollPropertyAtts.Length == 0)
                configCollPropertyAtts = GetAttributes<ConfigurationCollectionAttribute>(property.PropertyType);
            if (configCollPropertyAtts.Length == 0)
                return;

            var configAttribute = configPropertyAtts[0];
            var configCollAttribute = configCollPropertyAtts[0];

            XmlSchemaComplexType ct;
            var typeAlreadyInSchema = false;
            if (Generator.ComplexMap.ContainsKey(property.PropertyType))
            {
                //already done the work
                typeAlreadyInSchema = true;
                ct = Generator.ComplexMap[property.PropertyType];
            }
            else
            {
                //  got to generate a new one for the collection
                ct = new XmlSchemaComplexType
                {
                    Name = configAttribute.Name + "CT",
                };

                var seq = new XmlSchemaChoice
                {
                    MinOccurs = 0,
                    MaxOccursString = "unbounded"
                };
                ct.Particle = seq;
                Generator.ComplexMap.Add(property.PropertyType, ct);
                Generator.Schema.Items.Add(ct);
            }

            var element = new XmlSchemaElement
            {
                Name = configAttribute.Name,
                MinOccurs = configAttribute.IsRequired ? 1 : 0,
                SchemaTypeName = new XmlQualifiedName(XmlHelper.PrependNamespaceAlias(ct.Name))
            };
            var pct = (XmlSchemaComplexType) type;
            var items = ((XmlSchemaGroupBase) pct.Particle).Items;

            if (items.OfType<XmlSchemaElement>().All(x => x.Name != element.Name))
                items.Add(element);

            //  get all properties from the configuration object
            foreach (var pi in GetProperties<ConfigurationPropertyAttribute>(property.PropertyType))
            {
                Debug.WriteLine("ConfigurationProperty: " + pi.Name);
                var parser = TypeParserFactory.GetParser(Generator, pi);
                parser.GenerateSchemaTypeObjects(pi, ct, level + 1);
            }

            //  add the documentation
            element.AddAnnotation(property, configPropertyAtts[0]);

            //  at this point the element has been added to the schema
            //  but we now need to add support for the child elements
            if (!typeAlreadyInSchema)
            {
                AddCollectionChildren((XmlSchemaGroupBase) ct.Particle, configCollAttribute, level);
            }
        }

        /// <summary>
        ///     Adds the child elements of the collection
        /// </summary>
        protected void AddCollectionChildren(XmlSchemaGroupBase parentParticle, ConfigurationCollectionAttribute configCollAttribute, int level)
        {
            Debug.IndentLevel = level;

            XmlSchemaComplexType ct;
            if (Generator.ComplexMap.ContainsKey(configCollAttribute.ItemType))
            {
                //already done the work
                ct = Generator.ComplexMap[configCollAttribute.ItemType];
            }
            else
            {
                //  got to generate a new one for the collection
                ct = new XmlSchemaComplexType { Name = configCollAttribute.ItemType.Name + "CT" };
                ct.CreateSchemaSequenceParticle();

                Generator.ComplexMap.Add(configCollAttribute.ItemType, ct);
                Generator.Schema.Items.Add(ct);
            }

            //  ok we now have the child element as a complextype object
            //  but we need to add three elements to the parent complex type
            //  which support the add / remove / clear methods of the collection
            //  if we must add a clear method then it has to come first

            //  the basic map types do not include the clear and remove methods
            if (configCollAttribute.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMap ||
                configCollAttribute.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMapAlternate)
            {
                //  clear method (which is a simple element)
                var element = new XmlSchemaElement
                {
                    Name = configCollAttribute.ClearItemsName,
                    MinOccurs = 0,
                    MaxOccurs = 1,
                    //SchemaType = new XmlSchemaComplexType()
                };
                parentParticle.Items.Add(element);

                //  remove method
                element = new XmlSchemaElement
                {
                    MinOccurs = 0,
                    Name = configCollAttribute.RemoveItemName
                };

                var removeCt = new XmlSchemaComplexType();
                element.SchemaType = removeCt;

                //  get the type contained in the collection and work out
                //  what the key property is 
                var childElementType = configCollAttribute.ItemType;
                var found = false;
                foreach (var pi in childElementType.GetProperties())
                {
                    if (found)
                        break;

                    Debug.WriteLine("Child property: " + pi.Name);

                    var cpAtts = GetAttributes<ConfigurationPropertyAttribute>(pi);

                    // this should return a standardtypeparser object, if it doesnt
                    //  then it means that the key of the element was an element 
                    //  itself and I don't think that is possible!

                    if (cpAtts.Any(att => att.IsKey))
                    {
                        var parser = TypeParserFactory.GetParser(Generator, pi);
                        parser.GenerateSchemaTypeObjects(pi, removeCt, level + 1);
                        found = true;
                    }
                }

                parentParticle.Items.Add(element);
                SetMaxOccurs(element, parentParticle);
            }

            //  add method
            var addElement = new XmlSchemaElement
            {
                Name = configCollAttribute.AddItemName,
                MinOccurs = 0,
                SchemaTypeName = new XmlQualifiedName(XmlHelper.PrependNamespaceAlias(ct.Name))
            };

            parentParticle.Items.Add(addElement);

            SetMaxOccurs(addElement, parentParticle);

            //  get all properties from the configuration object
            var propertyInfos = GetProperties<ConfigurationPropertyAttribute>(configCollAttribute.ItemType);
            foreach (var pi in propertyInfos)
            {
                var parser = TypeParserFactory.GetParser(Generator, pi);
                Debug.WriteLine("\tConfigurationProperty: {0} {1}", pi.Name, parser.GetType());
                parser.GenerateSchemaTypeObjects(pi, ct, level + 1);
            }
        }

        private static void SetMaxOccurs(XmlSchemaElement element, XmlSchemaObject parent)
        {
            if (parent is XmlSchemaAll)
                element.MaxOccurs = 1;
            else
                element.MaxOccursString = "unbounded";
        }
    }
}