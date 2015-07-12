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
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using JFDI.Utils.XSDExtractor.Parsers;

namespace JFDI.Utils.XSDExtractor
{
    /// <summary>
    ///     Responsible for generating a <see cref="XmlSchema" /> object based
    ///     on the content of a <see cref="Type" /> object.
    /// </summary>
    public class XsdGenerator
    {
        private readonly Type _configType;
        private XmlSchemaElement _rootElement;
        private XmlSchema _schemaDoc;

        /// <summary>
        ///     Creates an instance of the XsdGenerator class
        /// </summary>
        /// <param name="configType">The is the type which should be examined and converted to an Xsd</param>
        public XsdGenerator(Type configType)
        {
            _configType = configType;
            ComplexMap = new Dictionary<Type, XmlSchemaComplexType>();
        }

        /// <summary>
        ///     Gets the actual schema object that is created by
        ///     parsing the <see cref="_configType" /> object
        /// </summary>
        public XmlSchema Schema
        {
            get
            {
                if (_schemaDoc != null) 
                    return _schemaDoc;
                else
                {
                    _schemaDoc = new XmlSchema
                    {
                        ElementFormDefault = XmlSchemaForm.Qualified
                    };

                    if (XmlHelper.UseTargetNamespace == null)
                    {
                        _schemaDoc.TargetNamespace = _configType.ToString().ToLower().StartsWith("http://")
                            ? _configType.ToString()
                            : "http://" + _configType;
                    }

                    else if (XmlHelper.UseTargetNamespace != "")
                    {
                        _schemaDoc.TargetNamespace = XmlHelper.UseTargetNamespace;
                    }
                    return _schemaDoc;
                }
            }
        }

        /// <summary>
        ///     Complex map property which tells us which complex types
        ///     have already been created
        /// </summary>
        public Dictionary<Type, XmlSchemaComplexType> ComplexMap { get; private set; }

        /// <summary>
        ///     Creates the root element for the schema.
        /// </summary>
        protected XmlSchemaElement CreateRootElement(string rootElementName)
        {
            //  create the actual root element, use the name of the 
            //  config type object to name it
            _rootElement =
                XmlHelper.CreateElement(string.IsNullOrEmpty(rootElementName) ? _configType.Name : rootElementName);

            //  this is the type that the root element is made up of
            var ct = XmlHelper.CreateComplexType(_rootElement.Name);
            Schema.Items.Add(ct);

            ct.AddAnnotation(_configType, null);
            //  add the all extension to the complex type so that child elements 
            //  may occur in any order
            XmlHelper.CreateSchemaSequenceParticle(ct);

            //  finally assign the type to the root element
            //  and add it to the document
            _rootElement.SchemaTypeName = new XmlQualifiedName((XmlHelper.UseTargetNamespace == null ? XmlHelper.TargetNamespaceAlias : String.Empty) + _rootElement.Name + "CT");
            Schema.Items.Add(_rootElement);

            return _rootElement;
        }

        /// <summary>
        ///     Generates a new XmlSchema object from the <see cref="_configType" /> object
        /// </summary>
        public XmlSchema GenerateXsd(string rootElementName)
        {
            _rootElement = CreateRootElement(rootElementName);
            var rootCt = (XmlSchemaComplexType) _schemaDoc.Items[0];

            //  get all properties from the configuration object
            var properties = TypeParser.GetProperties<ConfigurationPropertyAttribute>(_configType);
            foreach (var pi in properties)
            {
                Debug.IndentLevel = 0;
                Debug.WriteLine("ConfigurationProperty: " + pi.Name);

                var parser = TypeParserFactory.GetParser(this, pi);
                parser.GenerateSchemaTypeObjects(pi, rootCt, 0);
            }

            return _schemaDoc;
        }
    }
}