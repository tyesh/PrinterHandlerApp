using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace print_handler
{
    internal class Program
    {
        private const string _uriPrefix = "printHandler";

        /// <summary>
        /// C# application to provide print method for default printer with a custom URI scheme
        /// </summary>
        /// <param name="configureUriScheme">Flag to ask the application to configure the URI scheme</param>
        /// <param name="documentId">Id for the document to print</param>
        /// <returns></returns>
        static void Main(
            bool configureUriScheme = false,
            string documentId = ""
        )
        {
            if(configureUriScheme) {
                if(ConfigureURIScheme()) {
                    System.Console.WriteLine("Successfully registered the URI scheme");
                } else {
                    System.Console.WriteLine("Failed to configure the URI scheme");
                }
            }

            System.Console.WriteLine("Done");
        }

        /// <summary>
        /// Configure a URI scheme for any supported platform
        /// </summary>
        /// <returns>true for success</returns>
        private static bool ConfigureURIScheme() {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return ConfigureURISchemeWindows();
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Configure a URI scheme for Windows, using the register
        /// </summary>
        /// <returns>true for success</returns>
        private static bool ConfigureURISchemeWindows() {
            string assemblyLocation = Assembly.GetEntryAssembly().Location;

            if(Path.GetExtension(assemblyLocation).Equals(".dll", StringComparison.OrdinalIgnoreCase)) {
                assemblyLocation = Path.ChangeExtension(assemblyLocation, ".exe");

                if(!File.Exists(assemblyLocation)) {
                    System.Console.WriteLine("Could not find executable, please package as an exe.");
                    return false;
                }
            }

            try
            {
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(_uriPrefix)) {
                    key.SetValue(string.Empty, "URL: Custom print protocol");
                    key.SetValue("URL protocol", string.Empty);

                    using (RegistryKey shellkey = key.CreateSubKey("shell"))
                    using (RegistryKey openKey = shellkey.CreateSubKey("open"))
                    using (RegistryKey commandKey = openKey.CreateSubKey("command")) {
                        commandKey.SetValue(string.Empty, $"\"{assemblyLocation}\" --document-id \"%i\"");
                    }
                }

                return true;
            }
            catch (UnauthorizedAccessException authEx)
            {
                System.Console.WriteLine($"Failed to register the URI scheme: {authEx.Message}");
            }

            return false;
        }

    }
}