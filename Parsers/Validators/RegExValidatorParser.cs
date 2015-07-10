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

namespace JFDI.Utils.XSDExtractor.Parsers.Validators {
  
  /// <summary>
  /// 
  /// </summary>
  public class RegExValidatorParser : NoValidatorParser {

    /// <summary>
    /// 
    /// </summary>
    public RegExValidatorParser(PropertyInfo property, ConfigurationValidatorAttribute attribute)
      : base(property, attribute) {
    }

    /// <summary>
    /// Returns a simple type which extends the basic datatype and
    /// restricts it using a regex pattern
    /// </summary>
    public override XmlSchemaSimpleType GetSimpleType(string attributeDataType) {

      XmlSchemaSimpleType retVal = base.GetSimpleType(attributeDataType);
      XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction)retVal.Content;

      RegexStringValidatorAttribute rxa = (RegexStringValidatorAttribute)attribute;
      XmlSchemaPatternFacet pFacet = new XmlSchemaPatternFacet();
      pFacet.Value = rxa.Regex;
      restriction.Facets.Add(pFacet);

      return retVal;

    }

  }

}