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
    ///     Serves as the base class for all classes which have
    ///     to deal with validator type attributes
    /// </summary>
    public abstract class ValidatorAttributeParser
    {
        protected ConfigurationValidatorAttribute Attribute { get; }
        protected PropertyInfo Property { get; }

        /// <summary>
        /// </summary>
        protected ValidatorAttributeParser(PropertyInfo property, ConfigurationValidatorAttribute attribute)
        {
            Property = property;
            Attribute = attribute;
        }

        /// <summary>
        ///     Returns a simple type which extends the basic datatype and
        ///     restricts is using the attribute data
        /// </summary>
        public abstract XmlSchemaSimpleType GetSimpleType(string attributeDataType);
    }
}