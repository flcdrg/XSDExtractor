using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses;

using NUnit.Framework;

namespace JFDI.Utils.XSDExtractor.UnitTests
{
    [TestFixture]
    public class CollectionTests
    {
        private string SchemaToString(XmlSchema schema)
        {
            var sw = new StringWriter();
            schema.Write(sw);
            return sw.ToString();
        }

        private string[] XmlResources()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(n => n.EndsWith(".xml") && n.Contains("MultipleItemsIn")).ToArray();
        }

        [Test]
        [TestCaseSource("XmlResources")]
        public void Verify(string resourceName)
        {
            //  build a standard configurationsection with properties (using the mock object)
            var enterpriseConfig = new EnterpriseConfig();
            XmlHelper.UseAll = true;

            var configType = enterpriseConfig.GetType();
            var generator = new XsdGenerator(configType);
            XmlSchema schema = generator.GenerateXsd(configType.FullName);
            var schemaXml = SchemaToString(schema);

            var schemas = new XmlSchemaSet();
            schemas.Add("http://JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses.EnterpriseConfig", XmlReader.Create(new StringReader(schemaXml)));
            schemas.CompilationSettings.EnableUpaCheck = true;
            schemas.Compile();

            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                var doc = new XmlDocument();
                doc.Load(stream);

                doc.Schemas.Add(schemas);

                Debug.WriteLine(doc.OuterXml);


                Debug.WriteLine("------------------");
                doc.Validate(((sender, args) =>
                {
                    Debug.WriteLine("{0} {1}", args.Message, args.Severity);
                    Assert.Fail(args.Message);
                }));
            }
        }

        private string[] TwoChildCollectionsNotRequiredConfigResources()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(n => n.EndsWith(".xml") && n.Contains("TwoChildCollectionsNotRequiredConfig")).ToArray();
        }

        [Test]
        [TestCaseSource("TwoChildCollectionsNotRequiredConfigResources")]
        public void TwoChildCollectionsNotRequiredConfig(string resourceName)
        {
            //  build a standard configurationsection with properties (using the mock object)
            var configElement = new TwoChildCollectionsNotRequiredConfig();
            XmlHelper.UseAll = true;

            var configType = configElement.GetType();
            var generator = new XsdGenerator(configType);
            XmlSchema schema = generator.GenerateXsd(configType.FullName);
            var schemaXml = SchemaToString(schema);

            Debug.WriteLine(schemaXml);

            var schemas = new XmlSchemaSet();
            schemas.Add(schema.TargetNamespace, XmlReader.Create(new StringReader(schemaXml)));
            schemas.CompilationSettings.EnableUpaCheck = true;
            schemas.Compile();

            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                var doc = new XmlDocument();
                doc.Load(stream);

                doc.Schemas.Add(schemas);

                Debug.WriteLine(doc.OuterXml);


                Debug.WriteLine("------------------");
                doc.Validate(((sender, args) =>
                {
                    Debug.WriteLine("{0} {1}", args.Message, args.Severity);
                    Assert.Fail(args.Message);
                }));
            }

        }
    }
}