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
using System.Xml.Schema;

using JFDI.Utils.XSDExtractor.Parsers.Validators;

namespace JFDI.Utils.XSDExtractor.Parsers
{
    /// <summary>
    /// </summary>
    public class StandardTypeParser : TypeParser
    {
        /// <summary>
        /// </summary>
        public StandardTypeParser(XSDGenerator generator)
            : base(generator)
        {
        }

        /// <summary>
        /// </summary>
        public override void GenerateSchemaTypeObjects(PropertyInfo property, XmlSchemaType type)
        {
            var atts = GetAttributes<ConfigurationPropertyAttribute>(property);
            if (atts.Length == 0)
                return;

            //   don't include these in the xsd
            if (atts[0].Name.ToLower() == "xmlns")
                return;

            //  this should be a simple built in type, but if we can't 
            //  convert the type to an xs:type then we simply output the
            //  attribute as a string.
            string attributeType;
            switch (property.PropertyType.FullName)
            {
                case "System.Boolean":
                    attributeType = "xs:boolean";
                    break;

                case "System.Int32":
                    attributeType = "xs:int";
                    break;

                case "System.Int64":
                    attributeType = "xs:long";
                    break;

                case "System.Single":
                    attributeType = "xs:float";
                    break;

                case "System.Double":
                    attributeType = "xs:double";
                    break;

                case "System.DateTime":
                    attributeType = "xs:dateTime";
                    break;

                case "System.TimeSpan":
                    attributeType = "xs:time";
                    break;

                default:
                    attributeType = "xs:string";
                    break;
            }

            var attribute = XmlHelper.CreateAttribute(atts[0].Name);
            attribute.SchemaType = AddRestriction(property, attributeType);
            attribute.Use = atts[0].IsRequired ? XmlSchemaUse.Required : XmlSchemaUse.Optional;

            var ct = (XmlSchemaComplexType) type;
            ct.Attributes.Add(attribute);

            //  add the documentation for this attribute
            AddAnnotation(property, attribute, atts[0]);
        }

        /// <summary>
        ///     If the property contains any ConfigurationValidator attributes
        ///     then we should constrain the simple type with them
        /// </summary>
        protected XmlSchemaSimpleType AddRestriction(PropertyInfo property, string attributeDataType)
        {
            var parser = ValidatorFactory.GetValidator(property);
            return parser.GetSimpleType(attributeDataType);
        }
    }
}