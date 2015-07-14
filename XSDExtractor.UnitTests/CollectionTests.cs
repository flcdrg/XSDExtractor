using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Schema;

using JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses;

using NSubstitute;

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
            return Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(n => n.EndsWith(".xml")).ToArray();
        }

        [Test]
        [TestCaseSource("XmlResources")]
        public void Verify(string resourceName)
        {
            //  build a standard configurationsection with properties (using the mock object)
            var dm = Substitute.For<EnterpriseConfig>();
            var enterpriseConfig = dm;
            XmlHelper.UseTargetNamespace = string.Empty;

            var generator = new XsdGenerator(enterpriseConfig.GetType());
            XmlSchema schema = generator.GenerateXsd("UnitTestRootElement");

            var schemaXml = SchemaToString(schema);


            var schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(schemaXml)));

            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                var doc = new XmlDocument();
                doc.Load(stream);

                //Debug.WriteLine(doc.OuterXml);

                doc.Schemas.Add(schemas);

                Debug.WriteLine("------------------");
                doc.Validate(((sender, args) =>
                {
                    //Assert.Fail(args.Message);
                    Debug.WriteLine("{0} {1}", args.Message, args.Severity);
                }));
            }
        }
    }
}