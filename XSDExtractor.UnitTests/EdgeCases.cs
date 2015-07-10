using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses;

using NSubstitute;

using NUnit.Framework;

namespace JFDI.Utils.XSDExtractor.UnitTests
{
    [TestFixture(Description = "Checks that extraction works correctly at the extremes of the design.")]
    public class EdgeCases
    {
        /// <summary>
        ///     Creates a namespacemanager object
        /// </summary>
        private XmlNamespaceManager CreateNamespaceManager(XmlDocument doc)
        {
            var nsm = new XmlNamespaceManager(doc.NameTable);
            nsm.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            return nsm;
        }

        /// <summary>
        ///     Converts the Schema object into a string
        /// </summary>
        private string SchemaToString(XmlSchema schema)
        {
            using (var memoryStream = new MemoryStream())
            {
                schema.Write(memoryStream);
                memoryStream.Position = 0;
                var reader = new StreamReader(memoryStream);
                return reader.ReadToEnd();
            }
        }

        [Test(Description = "Checks that we can handle a ConfigurationSection object without any properties.")]
        public void TestBlankConfig()
        {
            //  build a standard configurationsection with properties (using the mock object)
            var dm = Substitute.For<ConfigurationSection>();
            var cs = dm;
            var generator = new XSDGenerator(cs.GetType());
            var schema = generator.GenerateXSD("UnitTestRootElement");
            var schemaXml = SchemaToString(schema);

            //  load the schema into the document
            var doc = new XmlDocument();
            doc.LoadXml(schemaXml);

            //  see if we can find the root element
            var nsm = CreateNamespaceManager(doc);
            var rootNode = doc.SelectSingleNode("//xs:element[@name='UnitTestRootElement']", nsm);
            Assert.IsNotNull(rootNode, "rootNode was null");
        }

        [Test(Description = "Checks that we correctly create an xsd for complicated ConfigurationSections.")]
        public void TestComplicatedConfigurationSection()
        {
            var generator = new XSDGenerator(typeof(AppSettingsSection));
            var schema = generator.GenerateXSD("UnitTestRootElement");
            var schemaXml = SchemaToString(schema);

            //  load the schema into the document
            var doc = new XmlDocument();
            doc.LoadXml(schemaXml);
        }

        [Test(Description = "Checks that we correctly exclude the xmlns attribute.")]
        public void TestXmlNSAttribute()
        {
            var generator = new XSDGenerator(typeof(ConfigurationWithXmlNSAttribute));
            var schema = generator.GenerateXSD("UnitTestRootElement");
            var schemaXml = SchemaToString(schema);

            //  load the schema into the document
            var doc = new XmlDocument();
            doc.LoadXml(schemaXml);

            //  see if we can find the xmlns attribute
            var nsm = CreateNamespaceManager(doc);
            var xpath = new StringBuilder();
            xpath.Append("//xs:schema/xs:complexType/xs:attribute[@name='xmlns']");

            var attributeNode = doc.SelectSingleNode(xpath.ToString(), nsm);
            Assert.IsNull(attributeNode, "xmlns attribute was found");
        }
    }
}