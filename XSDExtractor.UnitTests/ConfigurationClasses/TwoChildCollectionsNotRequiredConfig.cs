using System;
using System.Configuration;

namespace JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses
{
    public class TwoChildCollectionsNotRequiredConfig : ConfigurationSection
    {
        [ConfigurationProperty("child", IsRequired = true)]
        public Child Daughter
        {
            get { return (Child)this["child"]; }
            set { this["child"] = value; }
        }

        public class Child : ConfigurationElement
        {
            [ConfigurationProperty("Members", IsDefaultCollection = true)]
            public PersonConfigCollection Members
            {
                get { return (PersonConfigCollection)base["Members"]; }
            }

            [ConfigurationProperty("OtherMembers", IsDefaultCollection = true)]
            public PersonConfigCollection OtherMembers
            {
                get { return (PersonConfigCollection)this["OtherMembers"]; }
            }
        }

    }

}