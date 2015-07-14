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
    ///     an Xsd Complex type when the collection element is actually the default collection
    ///     for its parent type.
    /// </summary>
    public class DefaultConfigurationCollectionParser : ConfigurationCollectionParser
    {
        /// <summary>
        ///     Creates an instance of the <see cref="ConfigurationElementCollection" /> class
        /// </summary>
        public DefaultConfigurationCollectionParser(XsdGenerator generator)
            : base(generator)
        {
        }

        /// <summary>
        ///     Convert the property into a schema object
        /// </summary>
        public override void GenerateSchemaTypeObjects(PropertyInfo property, XmlSchemaType type, int level)
        {
            Debug.IndentLevel = level;
            Debug.WriteLine("Default {0} {1} {2}", level, property.Name, type.Name);

            var configPropertyAtts = GetAttributes<ConfigurationPropertyAttribute>(property);
            if (configPropertyAtts.Length == 0)
                return;

            var element = new XmlSchemaElement();
            element.Name = configPropertyAtts[0].Name;
            var ct = new XmlSchemaComplexType();
            element.SchemaType = ct;
            
            var configCollPropertyAtts = GetAttributes<ConfigurationCollectionAttribute>(property);
            if (configCollPropertyAtts.Length == 0)
                configCollPropertyAtts = GetAttributes<ConfigurationCollectionAttribute>(property.PropertyType);
            if (configCollPropertyAtts.Length == 0)
                return;

            var configCollAttribute = configCollPropertyAtts[0];

            //  we are actually going to add the collection to the parent type by creating
            //  a new group type that consists of a sequence of all the elements that we
            //  expect in the collection
            var groupParticle = XmlHelper.CreateGroupType(property.DeclaringType.FullName + "." + property.PropertyType.Name);
            groupParticle.CreateSchemaSequenceParticle();

            //  add support for the child elements
            AddCollectionChildren(groupParticle.Particle, configCollAttribute, level);

            //  now add the group to the schema and the parent CT
            if (Generator.Schema.Items.OfType<XmlSchemaGroup>().All(x => x.Name != groupParticle.Name))
                Generator.Schema.Items.Add(groupParticle);

            var parentCt = (XmlSchemaComplexType) type;
            var groupRef = new XmlSchemaGroupRef
            {
                RefName = new XmlQualifiedName(XmlHelper.PrependNamespaceAlias(groupParticle.Name))
            };

            var items = ((XmlSchemaGroupBase) parentCt.Particle).Items;

            ct.Particle = groupRef;
            items.Add(element);

/*            if (items.OfType<XmlSchemaGroupRef>().All(x => x.RefName != groupRef.RefName))
                items.Add(groupRef);*/

            //  add the documentation
            groupRef.AddAnnotation(property, configPropertyAtts[0]);
        }
    }
}