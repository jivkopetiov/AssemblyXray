using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AssemblyXray
{
    public class MetaDataParser
    {
        private Assembly _assembly;

        public MetaData LoadMetadataFromFile(string filename)
        {
            _assembly = Assembly.LoadFrom(filename);

            var metadata = new MetaData();

            metadata.References = _assembly.GetReferencedAssemblies().Select(r => r.ToString()).ToArray();
            metadata.FullName = _assembly.GetName().FullName;
            metadata.Version = _assembly.GetName().Version.ToString();
            metadata.ClrVersion = CalculateClrVersion();
            metadata.Type = CalculateAssemblyType();
            metadata.AttributeValues = CalculateAttributes();
            metadata.Platform = CalculatePlatform();
            metadata.BuildType = CalculateBuildType();

            return metadata;
        }

        private BuildType CalculateBuildType()
        {
            var debuggableAttribute = GetAttribute<DebuggableAttribute>();
            if (debuggableAttribute != null)
            {
                return (debuggableAttribute.IsJITOptimizerDisabled || debuggableAttribute.IsJITTrackingEnabled) ? BuildType.Debug : BuildType.Release;
            }
            else
            {
                return BuildType.Release;
            }
        }

        private Platform CalculatePlatform()
        {
            var modules = _assembly.GetLoadedModules();

            if (modules == null || modules.Length == 0)
            {
                throw new InvalidOperationException("Failed to find a module in the assembly");
            }

            var module = modules[0];
            PortableExecutableKinds peKind;
            ImageFileMachine imageFileMachine;
            module.GetPEKind(out peKind, out imageFileMachine);
            return CalculatePlatform(peKind, imageFileMachine);
        }

        private Dictionary<string, string> CalculateAttributes()
        {
            var dictionary = new Dictionary<string, string>();

            var attributes = _assembly.GetCustomAttributes(false).Cast<Attribute>();

            foreach (var attribute in attributes)
            {
                var type = attribute.GetType();
                var properties = type.GetProperties().Where(p => p.Name != "TypeId").ToArray();

                string name = type.Name.Replace("Attribute", "");
                string value = "";

                if (!properties.Any())
                {
                    return new Dictionary<string, string>();
                }

                if (properties.Length == 1)
                {
                    var property = properties.Single();
                    object realValue = property.GetValue(attribute, null);

                    if (property.PropertyType == typeof(string))
                    {
                        value = "\"" + realValue.ToString() + "\"";
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        value = property.Name + "=" + realValue.ToString();
                    }
                    else
                    {
                        value = realValue.ToString();
                    }
                }
                else
                {
                    foreach (var property in properties)
                    {
                        object realValue = property.GetValue(attribute, null);
                        value += property.Name + "=" + realValue.ToString() + " & ";
                    }

                    value = value.Substring(0, value.Length - 3);
                }

                dictionary.Add(name, value);
            }

            return dictionary;
        }

        private AssemblyType CalculateAssemblyType()
        {
            var referenceNames = _assembly.GetReferencedAssemblies().Select(r => r.Name).ToArray();

            if (_assembly.EntryPoint == null)
            {
                if (referenceNames.Contains("System.Windows") &&
                    referenceNames.Contains("System.Windows.Browser"))
                {
                    return AssemblyType.Silverlight;
                }

                if (referenceNames.Contains("System.Web"))
                {
                    return AssemblyType.ASPNET;
                }

                return AssemblyType.ClassLibrary;
            }

            if (referenceNames.Contains("PresentationFramework") &&
                referenceNames.Contains("System.Xaml"))
            {
                return AssemblyType.WPF;
            }

            if (referenceNames.Contains("System.Windows.Forms"))
            {
                return AssemblyType.WindowsForms;
            }

            return AssemblyType.ConsoleApp;
        }

        private ClrVersion CalculateClrVersion()
        {
            string version = _assembly.ImageRuntimeVersion;

            switch (version[1])
            {
                case '1': return ClrVersion.One;
                case '2': return ClrVersion.Two;
                case '4': return ClrVersion.Four;
                default: throw new InvalidOperationException("Unknown Clr version: " + version);
            }
        }

        private T GetAttribute<T>() where T : Attribute
        {
            var attributes = (T[])_assembly.GetCustomAttributes(typeof(T), false);

            return attributes.FirstOrDefault();
        }

        private Platform CalculatePlatform(PortableExecutableKinds peKind, ImageFileMachine imageFileMachine)
        {
            if ((peKind & PortableExecutableKinds.Required32Bit) == PortableExecutableKinds.Required32Bit)
            {
                return Platform.x86;
            }
            else if ((peKind & PortableExecutableKinds.PE32Plus) == PortableExecutableKinds.PE32Plus)
            {
                return Platform.x64;
            }
            else
            {
                return Platform.AnyCpu;
            }
        }
    }
}

