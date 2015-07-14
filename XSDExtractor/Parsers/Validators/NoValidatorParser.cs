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
using System.Xml;
using System.Xml.Schema;

namespace JFDI.Utils.XSDExtractor.Parsers.Validators
{
    /// <summary>
    ///     This class deals with the cases when there aren't any validation attributes to
    ///     work with. It basically returns a normal unrestricted simpletype
    /// </summary>
    public class NoValidatorParser : ValidatorAttributeParser
    {
        /// <summary>
        /// </summary>
        public NoValidatorParser(PropertyInfo property)
            : this(property, null)
        {
        }

        /// <summary>
        /// </summary>
        protected NoValidatorParser(PropertyInfo property, ConfigurationValidatorAttribute attribute)
            : base(property, attribute)
        {
        }

        /// <summary>
        ///     Returns a SimpleType restricted by the datatype only
        /// </summary>
        public override XmlSchemaSimpleType GetSimpleType(string attributeDataType)
        {
            var retVal = new XmlSchemaSimpleType();

            // <xs:restriction base="<whatever xs:datatype is required>" />
            var dataTypeRestriction = new XmlSchemaSimpleTypeRestriction
            {
                BaseTypeName = new XmlQualifiedName(attributeDataType)
            };
            retVal.Content = dataTypeRestriction;

            //  if the property type is an enum then add an enumeration facet to the simple type
            if (Property.PropertyType.IsEnum)
            {
                // <xs:enumeration value="123" />
                foreach (var enumValue in Enum.GetNames(Property.PropertyType))
                {
                    var enumFacet = new XmlSchemaEnumerationFacet { Value = enumValue };
                    dataTypeRestriction.Facets.Add(enumFacet);
                }
            }

            return retVal;
        }
    }
}