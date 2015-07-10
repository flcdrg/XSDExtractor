using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses;
using System.Xml.Schema;
using NMock;
using System.IO;
using System.Xml;

namespace JFDI.Utils.XSDExtractor.UnitTests.Bugs {

  [TestFixture(Description = @"Recreates the bug identified by Idael Cardoso @ CodeProject 
                              where multiple uses of a single collection led to multiple complex types
                              being created which created an invalid schema.")]
  public class DuplicateCollectionTypes {

    [Test(Description="Checks if we re-use the complex types instead of re-creating them each time")]
    public void TestMultipleDefinitions() {

      //  build a standard configurationsection with properties (using the mock object)
      DynamicMock dm = new DynamicMock(typeof(EnterpriseConfig));
      EnterpriseConfig enterpriseConfig = (EnterpriseConfig)dm.MockInstance;
      XSDGenerator generator = new XSDGenerator(enterpriseConfig.GetType());
      XmlSchema schema = generator.GenerateXSD("UnitTestRootElement");
      string schemaXml = SchemaToString(schema);

      //  not sure why, but the add method throws an exception when called with
      //  the schema object above, we have to serialize it and then add it
      //  again - seems crazy but works.
      //compileSet.Add(schema);
      XmlSchemaSet compileSet = new XmlSchemaSet();
      schema = compileSet.Add(schema.TargetNamespace, new XmlTextReader(new StringReader(schemaXml)));
      compileSet.Compile();

    }

    /// <summary>
    /// Converts the Schema object into a string
    /// </summary>
    private string SchemaToString(XmlSchema schema) {

      StringWriter sw = new StringWriter();
      schema.Write(sw);
      return sw.ToString();

    }

  }

}