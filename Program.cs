using System;
using System.Reflection;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using PrinterUtility;
using System.Drawing.Printing;
using System.Management;

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

            printEpsonTestPage();

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

        private static void printEpsonTestPage() {
            PrinterUtility.EscPosEpsonCommands.EscPosEpson printer = new PrinterUtility.EscPosEpsonCommands.EscPosEpson();
            var BytesValue = printer.Separator();
            BytesValue = PrintExtensions.AddBytes(BytesValue, printer.CharSize.DoubleHeight6());
            BytesValue = PrintExtensions.AddBytes(BytesValue, printer.FontSelect.FontA());
            BytesValue = PrintExtensions.AddBytes(BytesValue, printer.Alignment.Center());
            BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("Title\n"));
            BytesValue = PrintExtensions.AddBytes(BytesValue, CutPage());
            PrinterUtility.PrintExtensions.Print(BytesValue, GetPrinterPort());
            System.Console.WriteLine("Press any key");
            System.Console.ReadLine();
        }

        private static byte[] CutPage() {
            List<byte> list = new List<byte>();
            list.Add(Convert.ToByte(Convert.ToChar(0x1D)));
            list.Add(Convert.ToByte('V'));
            list.Add((byte)66);
            list.Add((byte)3);
            return list.ToArray();
        }

        private static string GetDefaultPrinter() {
            PrinterSettings settings = new PrinterSettings();
            foreach (string printer in PrinterSettings.InstalledPrinters) {
                settings.PrinterName = printer;
                System.Console.WriteLine(printer);
                if (settings.IsDefaultPrinter)  {
                    return printer;
                }
            }
            return "";
        }

        private static string GetPrinterPort() {
            string query = String.Format("Select * from Win32_Printer WHERE Name LIKE '%{0}'", GetDefaultPrinter());
            ManagementObjectSearcher printers = new ManagementObjectSearcher(query);
            foreach(ManagementObject printer in printers.Get()) {
                System.Console.WriteLine((string) printer.GetPropertyValue("name"));
                System.Console.WriteLine((string) printer.GetPropertyValue("PortName"));
                System.Console.WriteLine((string) printer.GetPropertyValue("ShareName"));
                System.Console.WriteLine((string) printer.GetPropertyValue("ServerName"));
                System.Console.WriteLine((string) printer.GetPropertyValue("SystemName"));
                System.Console.WriteLine((string) printer.GetPropertyValue("Location"));
                //string printerName = "\\\\" + printer.GetPropertyValue("Location") + "\\" + printer.GetPropertyValue("name");
                //EPSON-LX-350 @ atorales-MS-7C31
                string printerName = (string) printer.GetPropertyValue("PortName");
                System.Console.WriteLine(printerName);
                return printerName;
            }
            return "";
        }

    }
}