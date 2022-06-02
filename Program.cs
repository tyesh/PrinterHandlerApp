using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Drawing.Printing;
using System.Drawing;

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
                    Console.WriteLine("Successfully registered the URI scheme");
                } else {
                    Console.WriteLine("Failed to configure the URI scheme");
                }
            }

            printGenericTestPage();
            Console.WriteLine("Done");
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
                    Console.WriteLine("Could not find executable, please package as an exe.");
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
                Console.WriteLine($"Failed to register the URI scheme: {authEx.Message}");
            }

            return false;
        }

        private static void printGenericTestPage() {

            string s = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer ac pellentesque est, id tristique quam. Proin vitae justo elit. Duis consectetur justo sed turpis pretium, ut viverra tellus porttitor. Donec tristique purus quis mi lobortis finibus. Integer in lectus nec nibh pellentesque malesuada vel et purus. Pellentesque porttitor tempus commodo. Mauris in est pulvinar, mattis leo a, posuere lorem. Etiam egestas libero ac arcu suscipit fermentum. Sed auctor fringilla urna, id fermentum odio vestibulum a. Suspendisse potenti. Donec elit magna, luctus non aliquet nec, scelerisque sed tortor. Morbi hendrerit interdum nulla id placerat. Donec libero leo, ornare ac ullamcorper eu, eleifend sed urna. Curabitur gravida fringilla augue ut euismod.";

            PrintDocument p = new PrintDocument();
            p.PrintPage += delegate(Object sender1, PrintPageEventArgs e1) {
                e1.Graphics.DrawString(s, new Font("TImes New Roman", 12), new SolidBrush(Color.Black), new RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
            };
            try
            {
               p.Print(); 
            }
            catch (Exception ex)
            {
                throw new Exception("Exception ocurred while printing", ex);
            }
        }

    }
}