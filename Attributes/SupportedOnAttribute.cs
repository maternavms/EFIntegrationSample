using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEfMultipleSqlVersions.Attributes
{
    public abstract class SupportedAttribute: Attribute
    {
        // This is a positional argument
        protected SupportedAttribute(Version version)
        {
            Version = version;
        }
        public Version Version { get; } = new Version(999, 999, 999);
        public abstract bool IsSupported(Version version);        
    }


    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SupportedFromAttribute : SupportedAttribute
    {
        public SupportedFromAttribute(SqlServerVersion version) : base(new Version((int)version, 0))
        {
        }

        public SupportedFromAttribute(Version version): base(version)
        {
        }
        override public bool IsSupported(Version version)
        {
            return version >= Version;
        }
    }


    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SupportedOnAttribute : SupportedAttribute
    {
        public SupportedOnAttribute(SqlServerVersion version): base(new Version((int)version, 0))
        {
        }

        public SupportedOnAttribute(Version version): base(version)
        {
        }

        override public bool IsSupported(Version version)
        {
            return version == Version;
        }
    }

    public class SupportedUptoAttribute : SupportedAttribute
    {
        public SupportedUptoAttribute(SqlServerVersion version) : base(new Version((int)version, 0))
        {
        }

        public SupportedUptoAttribute(Version version): base(version)
        {
        }

        override public bool IsSupported(Version version)
        {
            return version <= Version;
        }
    }

    
    
}
