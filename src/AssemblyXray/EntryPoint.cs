using System;
using System.IO;
using System.Linq;
using NDesk.Options;

namespace AssemblyXray
{
    public class EntryPoint
    {
        static void Main(string[] args)
        {
#if DEBUG
            if (args == null || args.Length == 0)
            {
                args = new[] { Path.Combine(Environment.CurrentDirectory, "AssemblyXRay.exe") };
            }
#endif

            string outputFile = null;
            var argumentOptions = new OptionSet() {
                { "of|outputfile=", v => outputFile = v }
            };

            var arguments = argumentOptions.Parse(args);

            if (arguments == null || arguments.Count == 0)
            {
                PrintUsage();
                return;
            }

            string filePath = arguments.First();

            try
            {
                var parser = new MetaDataParser();
                var metadata = parser.LoadMetadataFromFile(filePath);

                if (string.IsNullOrEmpty(outputFile))
                {
                    Console.WriteLine(metadata.ToPrettyString());
                }
                else
                {
                    File.WriteAllText(outputFile, metadata.ToPrettyString());
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("xray.exe <AssemblyFile> [-of={PathToOutputFile}]");
        }

        private static void HandleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;

#if DEBUG
            Console.WriteLine(ex.ToString());
#else 
                Console.WriteLine(ex.GetType().Name + ": " + ex.Message);
#endif

            Console.ResetColor();
        }
    }
}
