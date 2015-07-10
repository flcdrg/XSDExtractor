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

namespace JFDI.Utils.XSDExtractor.Parsers.Validators {

  public static class ValidatorFactory {

    /// <summary>
    /// Returns a validator object which can convert a configurationvalidator
    /// attribute into a restricted SimpleType
    /// </summary>
    public static ValidatorAttributeParser GetValidator(PropertyInfo property) {

      foreach (ConfigurationValidatorAttribute att in property.GetCustomAttributes(typeof(ConfigurationValidatorAttribute), true)) {

        if (att is StringValidatorAttribute) {
          return new StringValidatorParser(property, att);
        }
        else if (att is RegexStringValidatorAttribute) {
          return new RegExValidatorParser(property, att);
        }
        else if (att is IntegerValidatorAttribute) {
          return new IntegerValidatorParser(property, att);
        }
        else if (att is LongValidatorAttribute) {
          return new LongValidatorParser(property, att);
        }
        else if (att is PositiveTimeSpanValidatorAttribute) {
          return new PositiveTimeSpanValidatorParser(property, att);
        }
       
        /*
        * Couldn't think of a way to restrict these validators in an Xsd
        * so they (for now at least) aren't added as any sort of restriction
        * 
        else if (att is TimeSpanValidatorAttribute) {
        }
        else if (att is SubclassTypeValidatorAttribute) {
        }
        */

      }

      //  no validators were found, so use a standard simple type instead
      return new NoValidatorParser(property);

    }

  }

}