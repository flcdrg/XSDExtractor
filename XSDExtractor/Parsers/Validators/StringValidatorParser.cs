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

namespace JFDI.Utils.XSDExtractor.Parsers.Validators
{
    /// <summary>
    /// </summary>
    public class StringValidatorParser : NoValidatorParser
    {
        /// <summary>
        /// </summary>
        public StringValidatorParser(PropertyInfo property, ConfigurationValidatorAttribute attribute)
            : base(property, attribute)
        {
        }

        /// <summary>
        ///     Returns a simple type which extends the basic datatype and
        ///     restricts is using the string validator
        /// </summary>
        public override XmlSchemaSimpleType GetSimpleType(string attributeDataType)
        {
            var retVal = base.GetSimpleType(attributeDataType);
            var restriction = (XmlSchemaSimpleTypeRestriction) retVal.Content;

            var sva = (StringValidatorAttribute) Attribute;
            if (!string.IsNullOrEmpty(sva.InvalidCharacters))
            {
                var pFacet = new XmlSchemaPatternFacet { Value = sva.InvalidCharacters };
                // TODO: convert this to a regex that excludes the characters
                pFacet.Value = string.Format(
                    "[^{0}]*",
                    pFacet.Value
                        .Replace(@"\", @"\\")
                        .Replace(@"[", @"\[")
                        .Replace(@"]", @"\]"));

                restriction.Facets.Add(pFacet);
            }

            var minFacet = new XmlSchemaMinLengthFacet { Value = sva.MinLength.ToString() };
            restriction.Facets.Add(minFacet);

            var maxFacet = new XmlSchemaMaxLengthFacet { Value = sva.MaxLength.ToString() };
            restriction.Facets.Add(maxFacet);

            return retVal;
        }
    }
}