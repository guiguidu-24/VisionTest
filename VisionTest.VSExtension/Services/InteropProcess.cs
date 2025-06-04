using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace VisionTest.VSExtension.Services
{
    public class InteropProcess : Process
    {
        private static readonly string fileName = GetFileName();

        public InteropProcess() : base()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            EnableRaisingEvents = true;

        }

        private static string GetFileName()
        {
            // Get the path of the executing assembly (the VSIX extension)
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string extensionDirectory = Path.GetDirectoryName(assemblyPath);

            // Path to where the files are deployed by the VSIX engine
            string interopFolder = Path.Combine(extensionDirectory, "InteropTools");

            var path = Path.Combine(interopFolder, "VisionTest.ConsoleInterop.exe");

            if (!File.Exists(path))
                throw new FileNotFoundException("The interop console executable was not found.", path);

            // Full path to the console EXE
            return path;
        }
    }
}
