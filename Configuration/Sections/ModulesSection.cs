using System.Configuration;

namespace HtmlComparer.Configuration.Sections
{
    public class ModulesSection : ConfigurationSection
    {

        [ConfigurationProperty("comparers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ComparersCollection))]
        public ComparersCollection Comparers
        {
            get { return (ComparersCollection)base["comparers"]; }
            set
            {
                ComparersCollection comparersCollection = value;
            }
        }

        [ConfigurationProperty("checkers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CheckersCollection))]
        public CheckersCollection Checkers
        {
            get { return (CheckersCollection)base["checkers"]; }
        }
    }

    #region ComparersSection
    public class ComparersCollection : ConfigurationElementCollection
    {
        public CompareConfig this[int index]
        {
            get { return (CompareConfig)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CompareConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CompareConfig)element).Type;
        }
    }

    public class CompareConfig : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string Type
        {
            get { return (string)this["type"]; }
        }

        [ConfigurationProperty("constructorParamsSection")]
        public string ConstructorParamsSection
        {
            get { return (string)this["constructorParamsSection"]; }
        }      
    }
   
    #endregion

    #region CheckersSection
    public class CheckersCollection : ConfigurationElementCollection
    {
        public CheckerConfig this[int index]
        {
            get { return (CheckerConfig)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CheckerConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CheckerConfig)element).Type;
        }
    }

    public class CheckerConfig : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
        }

        [ConfigurationProperty("constructorParamsSection")]
        public string ConstructorParamsSection
        {
            get { return (string)this["constructorParamsSection"]; }
        }
    }
    #endregion
}


