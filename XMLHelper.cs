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

using System.Xml.Schema;

namespace JFDI.Utils.XSDExtractor {
  
  /// <summary>
  /// Static class which provides methods to assist 
  /// with XmlSchema type object manipulation / creation 
  /// </summary>
  public static class XMLHelper {
    
    /// <summary>
    /// All new types created with the XmlSchema classes
    /// are placed into a namespace with this alias by default
    /// </summary>
    public const string TargetNamespaceAlias = "tns:";

    /// <summary>
    /// Adds the <see cref="TargetNamespaceAlias"/> to the start of 
    /// the name if it doesn't already exist
    /// </summary>
    public static string PrependNamespaceAlias(string name) {

      if (name.StartsWith(TargetNamespaceAlias))
        return name;

      return TargetNamespaceAlias + name;

    }

    /// <summary>
    /// Creates a new element
    /// </summary>
    public static XmlSchemaElement CreateElement(string name) {

      XmlSchemaElement element = new XmlSchemaElement();
      element.Name = name;
      return element;

    }

    /// <summary>
    /// Creates a new attribute
    /// </summary>
    public static XmlSchemaAttribute CreateAttribute(string name) {

      XmlSchemaAttribute attribute = new XmlSchemaAttribute();
      attribute.Name = name;
      return attribute;

    }

    /// <summary>
    /// Creates a new complex type
    /// </summary>
    public static XmlSchemaComplexType CreateComplexType(string name) {

      XmlSchemaComplexType complexType = new XmlSchemaComplexType();
      complexType.Name = name + "CT";
      return complexType;

    }

    /// <summary>
    /// Creates a new group type
    /// </summary>
    public static XmlSchemaGroup CreateGroupType(string name) {

      XmlSchemaGroup groupType = new XmlSchemaGroup();
      groupType.Name = name + "GroupCT";
      return groupType;

    }

    /// <summary>
    /// Creates a new <see cref="XmlSchemaSequence"/> particle type and assigns
    /// it to the <paramref name="complexType"/> parameter
    /// </summary>
    public static XmlSchemaSequence CreateSchemaSequenceParticle(XmlSchemaComplexType complexType) {

      XmlSchemaSequence seq = new XmlSchemaSequence();
      complexType.Particle = seq;
      return seq;

    }

  }
  
}