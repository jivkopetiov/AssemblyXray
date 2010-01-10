using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace AssemblyXray.Tests
{
    [TestFixture]
    public class MetaDataParserUnitTests
    {
        private const string SampleAssembliesFolder = @"..\..\SampleAssemblies";

        [Test]
        public void VS2010_x86_Release_NET40ClientProfile()
        {
            var parser = new MetaDataParser();
            var metadata = parser.LoadMetadataFromFile(
                    Path.Combine(SampleAssembliesFolder, "VS2010_x86_Release_NET40ClientProfile.exe"));

            Assert.AreEqual(BuildType.Release, metadata.BuildType);
            Assert.AreEqual(Platform.x86, metadata.Platform);
            Assert.AreEqual(ClrVersion.Four, metadata.ClrVersion);
        }

        [Test]
        public void VS2010_AnyCpu_Debug_NET20()
        {
            var parser = new MetaDataParser();
            var metadata = parser.LoadMetadataFromFile(
                    Path.Combine(SampleAssembliesFolder, "VS2010_AnyCpu_Debug_NET20.exe"));

            Assert.AreEqual(BuildType.Debug, metadata.BuildType);
            Assert.AreEqual(Platform.AnyCpu, metadata.Platform);
            Assert.AreEqual(ClrVersion.Two, metadata.ClrVersion);
        }

        [Test]
        public void Determine_Wpf()
        {
            var parser = new MetaDataParser();
            var metadata = parser.LoadMetadataFromFile(
                    Path.Combine(SampleAssembliesFolder, "wpf.exe"));

            Assert.AreEqual(AssemblyType.WPF, metadata.Type);
        }

        [Test]
        public void Determine_Silverlight()
        {
            var parser = new MetaDataParser();
            var metadata = parser.LoadMetadataFromFile(
                    Path.Combine(SampleAssembliesFolder, "silverlight.dll"));

            Assert.AreEqual(AssemblyType.Silverlight, metadata.Type);
        }

        [Test]
        public void Determine_WinForms()
        {
            var parser = new MetaDataParser();
            var metadata = parser.LoadMetadataFromFile(
                    Path.Combine(SampleAssembliesFolder, "winforms.exe"));

            Assert.AreEqual(AssemblyType.WindowsForms, metadata.Type);
        }

        [Test]
        public void Determine_AspNet()
        {
            var parser = new MetaDataParser();
            var metadata = parser.LoadMetadataFromFile(
                    Path.Combine(SampleAssembliesFolder, "aspnet.dll"));

            Assert.AreEqual(AssemblyType.ASPNET, metadata.Type);
        }
    }
}
