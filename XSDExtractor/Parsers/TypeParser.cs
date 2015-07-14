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
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace JFDI.Utils.XSDExtractor.Parsers
{
    /// <summary>
    ///     Base class for all type parsers
    /// </summary>
    public abstract class TypeParser
    {
        protected XsdGenerator Generator { get; private set; }

        /// <summary>
        ///     Creates an instance of the TypeParser object. CTOR is protected
        ///     because only inheriting classes should be able to use this constructor
        /// </summary>
        protected TypeParser(XsdGenerator generator)
        {
            Generator = generator;
        }

        /// <summary>
        ///     Returns all properties found in the type which have either a <see cref="ConfigurationPropertyAttribute" />
        ///     or a <see cref="ConfigurationCollectionAttribute" /> attribute.
        /// </summary>
        public static PropertyInfo[] GetProperties<TAttributeType>(Type t) where TAttributeType : Attribute
        {
            //  get all properties that have the appropriate atrribute on them

            return (from pi in t.GetProperties() let atts = pi.GetCustomAttributes(typeof(TAttributeType), true) where atts.Length > 0 select pi).ToArray();
        }

        /// <summary>
        ///     Returns all attributes found in the propertyinfo object which match the generic type property
        /// </summary>
        public static TAttributeType[] GetAttributes<TAttributeType>(Type type) where TAttributeType : Attribute
        {
            var atts = type.GetCustomAttributes(typeof(TAttributeType), true);
            var retval = new TAttributeType[atts.Length];

            //  get all properties that have the appropriate atrribute on them
            var i = 0;
            foreach (var objAttr in atts)
            {
                retval[i++] = (TAttributeType) objAttr;
            }

            return retval;
        }

        /// <summary>
        ///     Returns all attributes found in the propertyinfo object which match the generic type property
        /// </summary>
        public static TAttributeType[] GetAttributes<TAttributeType>(PropertyInfo pi) where TAttributeType : Attribute
        {
            var atts = pi.GetCustomAttributes(typeof(TAttributeType), true);
            var retval = new TAttributeType[atts.Length];

            //  get all properties that have the appropriate atrribute on them
            var i = 0;
            foreach (var objAttr in atts)
            {
                retval[i++] = (TAttributeType) objAttr;
            }

            return retval;
        }

        /// <summary>
        ///     Abstract method which, when overriden in super classes, allows the
        ///     class to actually parse the <paramref name="property" /> and decide which
        ///     schema objects should be created
        /// </summary>
        public abstract void GenerateSchemaTypeObjects(PropertyInfo property, XmlSchemaType type, int level);
    }
}