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
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using JFDI.Utils.XSDExtractor.Parsers;

namespace JFDI.Utils.XSDExtractor
{
    /// <summary>
    ///     Static class which provides methods to assist
    ///     with XmlSchema type object manipulation / creation
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        ///     All new types created with the XmlSchema classes
        ///     are placed into a namespace with this alias by default
        /// </summary>
        public const string TargetNamespaceAlias = "tns:";

        public static string UseTargetNamespace;

        /// <summary>
        /// If true, use xs:all instead of xs:sequence
        /// </summary>
        public static bool UseAll { get; set; }

        /// <summary>
        ///     Adds the <see cref="TargetNamespaceAlias" /> to the start of
        ///     the name if it doesn't already exist
        /// </summary>
        public static string PrependNamespaceAlias(string name)
        {
            if (UseTargetNamespace == "" || name.StartsWith(TargetNamespaceAlias))
                return name;

            return TargetNamespaceAlias + name;
        }

        /// <summary>
        ///     Creates a new element
        /// </summary>
        public static XmlSchemaElement CreateElement(string name)
        {
            var element = new XmlSchemaElement { Name = name };
            return element;
        }

        /// <summary>
        ///     Creates a new attribute
        /// </summary>
        public static XmlSchemaAttribute CreateAttribute(string name)
        {
            var attribute = new XmlSchemaAttribute { Name = name };
            return attribute;
        }

        /// <summary>
        ///     Creates a new complex type
        /// </summary>
        public static XmlSchemaComplexType CreateComplexType(string name)
        {
            var complexType = new XmlSchemaComplexType { Name = name + "CT" };
            return complexType;
        }

        /// <summary>
        ///     Creates a new group type
        /// </summary>
        public static XmlSchemaGroup CreateGroupType(string name)
        {
            var groupType = new XmlSchemaGroup { Name = name + "GroupCT" };
            return groupType;
        }

        /// <summary>
        ///     Creates a new <see cref="XmlSchemaSequence" /> particle type and assigns
        ///     it to the <paramref name="complexType" /> parameter
        /// </summary>
        public static void CreateSchemaSequenceParticle(this XmlSchemaComplexType complexType)
        {
            var seq = UseAll ? (XmlSchemaGroupBase) new XmlSchemaAll() : new XmlSchemaSequence();
            complexType.Particle = seq;
        }

        public static void CreateSchemaSequenceParticle(this XmlSchemaGroup complexType)
        {
            var seq = UseAll ?  (XmlSchemaGroupBase) new XmlSchemaAll() : new XmlSchemaSequence();
            complexType.Particle = seq;
        }

        /// <summary>
        ///     Provides standard documentation for a type in the form of XmlSchemaDocumentation objects
        /// </summary>
        public static void AddAnnotation(this XmlSchemaAnnotated annotatedType, PropertyInfo property, ConfigurationPropertyAttribute configProperty)
        {
            annotatedType.Annotation = new XmlSchemaAnnotation();

            //  human documentation
            var descriptionAtts = TypeParser.GetAttributes<DescriptionAttribute>(property);

            var xmlDocumentation = property.GetXmlDocumentation();

            //  standard info
            string standardDesc;

            if (configProperty != null)
            {
                standardDesc = configProperty.IsRequired ? "Required" : "Optional";
                standardDesc += " " + property.PropertyType.FullName;
                standardDesc += " " +
                                (configProperty.DefaultValue.ToString() == "System.Object"
                                    ? ""
                                    : "[" + configProperty.DefaultValue + "]");
            }
            else
            {
                standardDesc = "";
            }

            var documentation = new XmlSchemaDocumentation();
            if (descriptionAtts.Length > 0)
            {
                documentation.Markup = TextToNodeArray(descriptionAtts[0].Description + " " + standardDesc);
            }
            else if (!String.IsNullOrEmpty(xmlDocumentation))
            {
                documentation.Markup = TextToNodeArray(xmlDocumentation);
            }
            else
            {
                documentation.Markup = TextToNodeArray(standardDesc);
            }

            //  machine documentation
            var appInfo = new XmlSchemaAppInfo
            {
                Markup = TextToNodeArray(String.Format("{0}{1}", property.DeclaringType.FullName, property.Name))
            };

            //  add the documentation to the object
            annotatedType.Annotation.Items.Add(documentation);
            annotatedType.Annotation.Items.Add(appInfo);
        }        
        
        public static void AddAnnotation(this XmlSchemaAnnotated annotatedType, Type type, ConfigurationPropertyAttribute configProperty)
        {
            annotatedType.Annotation = new XmlSchemaAnnotation();

            //  human documentation
            var descriptionAtts = TypeParser.GetAttributes<DescriptionAttribute>(type);

            var xmlDocumentation = type.GetXmlDocumentation();

            //  standard info
            string standardDesc;

            if (configProperty != null)
            {
                standardDesc = configProperty.IsRequired ? "Required" : "Optional";
                standardDesc += " " + type.FullName;
                standardDesc += " " +
                                (configProperty.DefaultValue.ToString() == "System.Object"
                                    ? ""
                                    : "[" + configProperty.DefaultValue + "]");
            }
            else
            {
                standardDesc = "";
            }

            var documentation = new XmlSchemaDocumentation();
            if (descriptionAtts.Length > 0)
            {
                documentation.Markup = TextToNodeArray(descriptionAtts[0].Description + " " + standardDesc);
            }
            else if (!String.IsNullOrEmpty(xmlDocumentation))
            {
                documentation.Markup = TextToNodeArray(xmlDocumentation);
            }
            else
            {
                documentation.Markup = TextToNodeArray(standardDesc);
            }

            //  machine documentation
            var appInfo = new XmlSchemaAppInfo
            {
                Markup = TextToNodeArray(type.FullName)
            };

            //  add the documentation to the object
            annotatedType.Annotation.Items.Add(documentation);
            annotatedType.Annotation.Items.Add(appInfo);
        }

        public static XmlNode[] TextToNodeArray(string text)
        {
            var doc = new XmlDocument();
            return new XmlNode[] { doc.CreateTextNode(text) };
        }
    }
}