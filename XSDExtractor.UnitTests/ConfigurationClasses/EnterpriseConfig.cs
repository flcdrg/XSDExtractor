using System;
using System.Configuration;

namespace JFDI.Utils.XSDExtractor.UnitTests.ConfigurationClasses
{
    /// <summary>
    ///     This class is here to support the <see cref="JFDI.Utils.XSDExtractor.UnitTests.Bugs.DuplicateCollectionTypes" />
    ///     unit tests.
    /// </summary>
    /// <remarks>
    ///     The class was supplied by Idael Cardoso @ CodeProject
    /// </remarks>
    public class EnterpriseConfig : ConfigurationSection
    {
        [ConfigurationProperty("Managers",
            IsRequired = true,
            IsDefaultCollection = false)]
        public PersonConfigCollection Managers
        {
            get { return (PersonConfigCollection) base["Managers"]; }
        }

        [ConfigurationProperty("Dept", IsRequired = true)]
        public DeparmentConfig Department
        {
            get { return (DeparmentConfig) this["dept"]; }
            set { this["dept"] = value; }
        }

        [ConfigurationProperty("Deparments",
            IsRequired = true,
            IsDefaultCollection = false)]
        public DeparmentConfigCollection Deparments
        {
            get { return (DeparmentConfigCollection) base["Deparments"]; }
        }
    }

    public class PersonConfig : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base["Name"]; }
        }

        [ConfigurationProperty("Colour", DefaultValue = ConsoleColor.Red)]
        public ConsoleColor ConsoleColor
        {
            get { return (ConsoleColor) this["Colour"]; }
            set { this["Colour"] = value; }
        }

        [ConfigurationProperty("Extra", DefaultValue = false)]
        public bool Extra
        {
            get { return (bool) this["extra"]; }
            set { this["extra"] = value; }
        }
    }

    [ConfigurationCollection(typeof(PersonConfig),
        AddItemName = "Person",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class PersonConfigCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PersonConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PersonConfig) element).Name;
        }
    }

    public class DeparmentConfig : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base["Name"]; }
        }

        /// <summary>
        /// Department colour
        /// </summary>
        [ConfigurationProperty("Colour", DefaultValue = ConsoleColor.Blue)]
        public ConsoleColor ConsoleColor
        {
            get { return (ConsoleColor)this["Colour"]; }
            set { this["Colour"] = value; }
        }

        [ConfigurationProperty("Members",
            IsRequired = true, IsDefaultCollection = true)]
        public PersonConfigCollection Members
        {
            get { return (PersonConfigCollection) base["Members"]; }
        }

        [ConfigurationProperty("OtherMembers",
            IsRequired = true, IsDefaultCollection = true)]
        public PersonConfigCollection OtherMembers
        {
            get { return (PersonConfigCollection) this["OtherMembers"]; }
        }
    }

    [ConfigurationCollection(typeof(DeparmentConfig),
        CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class DeparmentConfigCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DeparmentConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DeparmentConfig) element).Name;
        }
    }
}