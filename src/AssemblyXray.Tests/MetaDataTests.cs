using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AssemblyXray.Tests
{
    [TestFixture]
    public class MetaDataTests
    {
        [Test]
        public void ToPrettyString_Works()
        {
            var metadata = new MetaData();
            metadata.BuildType = BuildType.Debug;
            metadata.ClrVersion = "3.0";
            metadata.FullName = "Assembly.dll";
            metadata.Platform = Platform.x64;
            metadata.References = new[] { "mscorlib.dll" };
            metadata.Type = AssemblyType.ConsoleApp;
            metadata.Version = "1.0";

            string expected =
@"General:
    FullName = Assembly.dll
    Version = 1.0
    Platform = x64
    BuildType = Debug
    CLR Version = 3.0
    Type = ConsoleApp

References:
    mscorlib.dll

Attributes:
";

            Assert.AreEqual(expected, metadata.ToPrettyString());
        }
    }
}
