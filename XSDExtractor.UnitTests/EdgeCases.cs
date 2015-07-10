using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NMock;
using System.Configuration;
using System.Xml.Schema;
using System.IO;
using System.Xml;
using JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses;

namespace JFDI.Utils.XSDExtractor.UnitTests {

  [TestFixture(Description="Checks that extraction works correctly at the extremes of the design.")]
  public class EdgeCases {

    [Test(Description="Checks that we can handle a ConfigurationSection object without any properties.")]
    public void TestBlankConfig() {

      //  build a standard configurationsection with properties (using the mock object)
      DynamicMock dm = new DynamicMock(typeof(ConfigurationSection));
      ConfigurationSection cs = (ConfigurationSection)dm.MockInstance;
      XSDGenerator generator = new XSDGenerator(cs.GetType());
      XmlSchema schema = generator.GenerateXSD("UnitTestRootElement");
      string schemaXml = SchemaToString(schema);

      //  load the schema into the document
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(schemaXml);

      //  see if we can find the root element
      XmlNamespaceManager nsm = CreateNamespaceManager(doc);
      XmlNode rootNode = doc.SelectSingleNode("//xs:element[@name='UnitTestRootElement']", nsm);
      Assert.IsNotNull(rootNode, "rootNode was null");

    }

    [Test(Description = "Checks that we correctly exclude the xmlns attribute.")]
    public void TestXmlNSAttribute() {

      XSDGenerator generator = new XSDGenerator(typeof(ConfigurationWithXmlNSAttribute));
      XmlSchema schema = generator.GenerateXSD("UnitTestRootElement");
      string schemaXml = SchemaToString(schema);

      //  load the schema into the document
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(schemaXml);

      //  see if we can find the xmlns attribute
      XmlNamespaceManager nsm = CreateNamespaceManager(doc);
      StringBuilder xpath = new StringBuilder();
      xpath.Append("//xs:schema/xs:complexType/xs:attribute[@name='xmlns']");

      XmlNode attributeNode = doc.SelectSingleNode(xpath.ToString(), nsm);
      Assert.IsNull(attributeNode, "xmlns attribute was found");

    }

    [Test(Description = "Checks that we correctly create an xsd for complicated ConfigurationSections.")]
    public void TestComplicatedConfigurationSection() {

      XSDGenerator generator = new XSDGenerator(typeof(System.Configuration.AppSettingsSection));
      XmlSchema schema = generator.GenerateXSD("UnitTestRootElement");
      string schemaXml = SchemaToString(schema);

      //  load the schema into the document
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(schemaXml);

    }

    /// <summary>
    /// Creates a namespacemanager object
    /// </summary>
    private XmlNamespaceManager CreateNamespaceManager(XmlDocument doc) {

      XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
      nsm.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
      return nsm;

    }

    /// <summary>
    /// Converts the Schema object into a string
    /// </summary>
    private string SchemaToString(XmlSchema schema) {

      using (MemoryStream memoryStream = new MemoryStream()) {
        schema.Write(memoryStream);
        memoryStream.Position = 0;
        StreamReader reader = new StreamReader(memoryStream);
        return reader.ReadToEnd();
      }

    }

  }

}
