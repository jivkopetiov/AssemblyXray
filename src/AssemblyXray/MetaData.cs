using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace AssemblyXray
{
    public class MetaData
    {
        public MetaData()
        {
            AttributeValues = new Dictionary<string, string>();
        }

        public Dictionary<string, string> AttributeValues { get; set; }

        public BuildType BuildType { get; set; }

        public AssemblyType Type { get; set; }

        public string[] References { get; set; }

        public string ClrVersion { get; set; }

        public string Version { get; set; }

        public Platform Platform { get; set; }

        public string FullName { get; set; }

        public string ToPrettyString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("General:");
            builder.AppendLine("    FullName = " + FullName);
            builder.AppendLine("    Version = " + Version);
            builder.AppendLine("    Platform = " + Platform);
            builder.AppendLine("    BuildType = " + BuildType);
            builder.AppendLine("    CLR Version = " + ClrVersion);
            builder.AppendLine("    Type = " + Type);
            builder.AppendLine();

            builder.AppendLine("References:");
            foreach (string reference in References)
            {
                builder.AppendLine("    " + reference);
            }
            builder.AppendLine();

            builder.AppendLine("Attributes:");
            foreach (var pair in AttributeValues)
            {
                builder.AppendFormat("    {0,-24}: {1}", pair.Key, pair.Value);
                builder.AppendLine();
            }
            builder.AppendLine();

            builder.AppendLine();

            return builder.ToString();
        }
    }

    public enum Platform
    {
        x86,
        x64,
        AnyCpu,
        ia64
    }

    public enum BuildType
    {
        Debug,
        Release
    }

    public enum AssemblyType
    {
        ConsoleApp,
        WindowsForms,
        WPF,
        Silverlight,
        ClassLibrary,
        ASPNET,
        ASPNETMVC
    }
}
