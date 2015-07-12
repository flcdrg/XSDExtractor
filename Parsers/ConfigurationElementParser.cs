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

namespace JFDI.Utils.XSDExtractor.Parsers
{
    /// <summary>
    ///     Responsible for converting a ConfigurationElement into
    ///     XmlSchema objects
    /// </summary>
    public class ConfigurationElementParser : TypeParser
    {
        /// <summary>
        /// </summary>
        public ConfigurationElementParser(XsdGenerator generator)
            : base(generator)
        {
        }

        /// <summary>
        /// </summary>
        public override void GenerateSchemaTypeObjects(PropertyInfo property, XmlSchemaType type, int level)
        {
            var atts = GetAttributes<ConfigurationPropertyAttribute>(property);
            if (atts.Length == 0)
                return;

            XmlSchemaComplexType ct;
            if (Generator.ComplexMap.ContainsKey(property.PropertyType))
            {
                //already done the work
                ct = Generator.ComplexMap[property.PropertyType];
            }
            else
            {
                //  got to generate a new one
                ct = new XmlSchemaComplexType { Name = atts[0].Name + "CT" };
                ct.AddAnnotation(property, null);
                XmlHelper.CreateSchemaSequenceParticle(ct);

                Generator.ComplexMap.Add(property.PropertyType, ct);
                Generator.Schema.Items.Add(ct);

                //  get all properties from the configuration object
                var propertyInfos = GetProperties<ConfigurationPropertyAttribute>(property.PropertyType);

                foreach (var pi in propertyInfos)
                {
                    var parser = TypeParserFactory.GetParser(Generator, pi);
                    parser.GenerateSchemaTypeObjects(pi, ct, level + 1);
                }
            }

            var element = new XmlSchemaElement
            {
                Name = atts[0].Name, // property.PropertyType.Name + "CT",
                MinOccurs = atts[0].IsRequired ? 1 : 0,
                SchemaTypeName = new XmlQualifiedName(XmlHelper.PrependNamespaceAlias(ct.Name))
            };
            var pct = (XmlSchemaComplexType) type;
            ((XmlSchemaGroupBase) pct.Particle).Items.Add(element);

            //  add the documentation
            element.AddAnnotation(property, atts[0]);
        }
    }
}