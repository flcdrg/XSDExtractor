using System.IO;
using System.Xml;
using System.Xml.Schema;
using JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses;
using NSubstitute;
using NUnit.Framework;

namespace JFDI.Utils.XSDExtractor.UnitTests.Bugs
{
    [TestFixture(Description = @"Recreates the bug identified by Idael Cardoso @ CodeProject 
                              where multiple uses of a single collection led to multiple complex types
                              being created which created an invalid schema.")]
    public class DuplicateCollectionTypes
    {
        /// <summary>
        ///     Converts the Schema object into a string
        /// </summary>
        private string SchemaToString(XmlSchema schema)
        {
            var sw = new StringWriter();
            schema.Write(sw);
            return sw.ToString();
        }

        [Test(Description = "Checks if we re-use the complex types instead of re-creating them each time")]
        public void TestMultipleDefinitions()
        {
            //  build a standard configurationsection with properties (using the mock object)
            var dm = Substitute.For<EnterpriseConfig>();
            var enterpriseConfig = (EnterpriseConfig) dm;
            var generator = new XSDGenerator(enterpriseConfig.GetType());
            var schema = generator.GenerateXSD("UnitTestRootElement");
            var schemaXml = SchemaToString(schema);

            //  not sure why, but the add method throws an exception when called with
            //  the schema object above, we have to serialize it and then add it
            //  again - seems crazy but works.
            //compileSet.Add(schema);
            var compileSet = new XmlSchemaSet();
            schema = compileSet.Add(schema.TargetNamespace, new XmlTextReader(new StringReader(schemaXml)));
            compileSet.Compile();
        }
    }
}