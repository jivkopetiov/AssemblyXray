using System;
using System.IO;

namespace AssemblyXray
{
    public class EntryPoint
    {
        static void Main(string[] args)
        {
            args = new[] { @"D:\binaries\BackupWorker.exe" };

            #if DEBUG
                if (args == null || args.Length == 0)
                {
                    args = new[] { Path.Combine(Environment.CurrentDirectory, "AssemblyXRay.exe") };
                }
            #endif

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Missing path as the first argument to this console app");
                return;
            }

            try
            {
                var parser = new MetaDataParser();
                var metadata = parser.LoadMetadataFromFile(args[0]);

                Console.WriteLine(metadata.ToPrettyString());
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
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
