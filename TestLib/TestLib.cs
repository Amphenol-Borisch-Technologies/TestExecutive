using ABT.Test.TestExecutive.TestLib.Configuration;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib {
    [Flags]
    public enum EVENTS {
        // NOTE:  EVENTS are defined in order of criticality.
        // - EVENTS' ordering is crucial to correctly evaluating TestExec results.
        // - Reordering without consideration will break TestGroup & TestOperation evaluation logic.
        // NOTE: Per Microsoft's CoPilot, 'foreach (EVENTS events in Enum.GetValues(typeof(EVENTS)))' will iterate over EVENTS in their defined order.
        // - This is true with the below order and flag values, with most critical having lowest flag value; EMERGENCY_STOP = 0b0000_0001...INFORMATION = 0b0100_0000.
        // - However, initially I set their flags from most critical having highest flag value; EMERGENCY_STOP = 0b0100_0000...INFORMATION = 0b0000_0001.
        //   This caused the above foreach to iterate in flag value sequence, not definition order.
        // - When I confronted CoPilot with this info, it agreed that flag value overrides definition order.
        EMERGENCY_STOP = 0b0000_0001, // Most critical event.
        ERROR = 0b0000_0010,          // Second most critical event.
        CANCEL = 0b0000_0100,         // Third most critical event.
        UNSET = 0b0000_1000,          //   .
        FAIL = 0b0001_0000,           //   .
        PASS = 0b0010_0000,           //   .
        INFORMATION = 0b0100_0000     // Least critical event.
    }
    // NOTE:  If modifying EVENTS, update EventColors correspondingly.
    // - Every EVENT requires an associated Color.

    public static class TestLib {
        public static readonly Dictionary<EVENTS, Color> EventColors = new Dictionary<EVENTS, Color> {
            { EVENTS.EMERGENCY_STOP, Color.Fuchsia },
            { EVENTS.ERROR, Color.Aqua },
            { EVENTS.CANCEL, Color.Yellow },
            { EVENTS.UNSET, Color.Gray },
            { EVENTS.FAIL, Color.Red },
            { EVENTS.PASS, Color.Green },
            { EVENTS.INFORMATION, Color.White }
        };

        private static readonly System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        public static readonly String TestExecutiveFolder = configuration.AppSettings.Settings[nameof(TestExecutiveFolder)].Value;
        public static readonly String TestPlansFolder = configuration.AppSettings.Settings[nameof(TestPlansFolder)].Value;
        public static readonly String TestPlanDefinitionXSD = TestExecutiveFolder + @"\TestPlanDefinition.xsd";
        public static readonly String TestExecutiveDefinitionXML = TestExecutiveFolder + @"\TestExecDefinition.xml";
        public static readonly TestExecDefinition testExecDefinition = Serializing.DeserializeFromFile<TestExecDefinition>(xmlFile: $"{TestExecutiveDefinitionXML}");
        public static readonly String SPACES_2 = "  ";
        public static readonly Int32 PAD_RIGHT = 21;
        internal static readonly String TestExecutive = nameof(TestExecutive);
        // TODO:  Eventually; mitigate or eliminate writeable global objects; use passed parameters instead.
        public static String TestPlanDefinitionXML = null;
        public static TestPlanDefinition testPlanDefinition = null;
        public static Dictionary<String, Object> InstrumentDrivers = null;
        public static TestSequence testSequence = null;
        public static String UserName = null;
        public static CancellationTokenSource CTS_Cancel;
        public static CancellationTokenSource CTS_EmergencyStop;
        public static CancellationToken CT_Cancel;
        public static CancellationToken CT_EmergencyStop;

        public static String FormatMessage(String Label, String Message) { return $"{SPACES_2}{Label}".PadRight(PAD_RIGHT) + $": {Message}"; }

        public static String BuildDate(Version version) {
            DateTime Y2K = new DateTime(year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Local);
            return $"{Y2K + new TimeSpan(days: version.Build, hours: 0, minutes: 0, seconds: 2 * version.Revision):g}";
        }

        public static Dictionary<String, Object> GetInstrumentDriversTestExecDefinition() {
            Dictionary<String, Object> instrumentDrivers = new Dictionary<String, Object>();

            Object instrumentDriver = null;
            foreach (InstrumentTestExec instrumentTestExec in testExecDefinition.InstrumentsTestExec.InstrumentTestExec) {
                try {
                    if (!testPlanDefinition.TestSpace.Simulate) instrumentDriver = Activator.CreateInstance(Type.GetType(instrumentTestExec.NameSpacedClassName), new Object[] { instrumentTestExec.Address, instrumentTestExec.Detail });
                    instrumentDrivers.Add(instrumentTestExec.ID, instrumentDriver); // instrumentDriver is null if testPlanDefinition.TestSpace.Simulate.
                } catch (Exception e) {
                    StringBuilder stringBuilder = new StringBuilder().AppendLine();
                    const Int32 PR = 23;
                    stringBuilder.AppendLine($"Issue with {nameof(InstrumentTestExec)}:");
                    stringBuilder.AppendLine($"   {nameof(instrumentTestExec.ID)}".PadRight(PR) + $": {instrumentTestExec.ID}");
                    stringBuilder.AppendLine($"   {nameof(instrumentTestExec.Detail)}".PadRight(PR) + $": {instrumentTestExec.Detail}");
                    stringBuilder.AppendLine($"   {nameof(instrumentTestExec.Address)}".PadRight(PR) + $": {instrumentTestExec.Address}");
                    stringBuilder.AppendLine($"   {nameof(instrumentTestExec.NameSpacedClassName)}".PadRight(PR) + $": {instrumentTestExec.NameSpacedClassName}{Environment.NewLine}");
                    stringBuilder.AppendLine($"{nameof(Exception)} {nameof(Exception.Message)}(s):");
                    stringBuilder.AppendLine($"{e}{Environment.NewLine}");
                    throw new ArgumentException(stringBuilder.ToString());
                }
            }
            return instrumentDrivers;
        }

        public static HashSet<String> GetDerivedClassnames<T>() where T : class {
            try {
                Assembly assembly = Assembly.GetAssembly(typeof(T));
                Type baseType = typeof(T);
                List<Type> derivedTypes = assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract).ToList();
                return new HashSet<String>(derivedTypes.Select(t => t.Name));
            } catch (ReflectionTypeLoadException reflectionTypeLoadException) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"{nameof(ReflectionTypeLoadException)}: '{reflectionTypeLoadException.Message}'.");
                foreach (Exception exception in reflectionTypeLoadException.LoaderExceptions) {
                    stringBuilder.AppendLine($"\tMessage: '{exception.Message}'.");
                    if (exception is TypeLoadException typeLoadException) stringBuilder.AppendLine($"\tTypeName: '{typeLoadException.TypeName}'.");
                }
                throw new Exception(stringBuilder.ToString());
            }
        }

        public static Dictionary<String, Object> GetInstrumentDriversTestPlanDefinition() {
            Dictionary<String, Object> instrumentDrivers = GetMobileTestPlanDefinition();
            foreach (KeyValuePair<String, Object> kvp in GetStationaryTestPlanDefinition()) instrumentDrivers.Add(kvp.Key, kvp.Value);
            if (!testPlanDefinition.TestSpace.Simulate) {
                foreach (KeyValuePair<String, Object> kvp in instrumentDrivers)
                    if (kvp.Value is IInstrument iInstrument) try {
                            iInstrument.ResetClear();
                        } catch {
                            _ = MessageBox.Show($"{iInstrument.Detail}{Environment.NewLine}" +
                                $"{iInstrument.Address}{Environment.NewLine}{Environment.NewLine}" +
                                $"Is not responding.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            throw;
                        }
            }
            return instrumentDrivers;
        }

        private static Dictionary<String, Object> GetMobileTestPlanDefinition() {
            Dictionary<String, Object> instrumentDrivers = new Dictionary<String, Object>();
            Object instrumentDriver = null;
            foreach (Mobile mobile in testPlanDefinition.InstrumentsTestPlan.Mobile) try {
                    if (!testPlanDefinition.TestSpace.Simulate) instrumentDriver = Activator.CreateInstance(Type.GetType(mobile.NameSpacedClassName), new Object[] { mobile.Address, mobile.Detail });
                    instrumentDrivers.Add(mobile.ID, instrumentDriver); // instrumentDriver is null if testPlanDefinition.TestSpace.Simulate.
                } catch (Exception exception) {
                    StringBuilder stringBuilder = new StringBuilder().AppendLine();
                    const Int32 PR = 23;
                    stringBuilder.AppendLine($"Issue with {nameof(Mobile)}:");
                    stringBuilder.AppendLine($"   {nameof(mobile.ID)}".PadRight(PR) + $": {mobile.ID}");
                    stringBuilder.AppendLine($"   {nameof(mobile.Detail)}".PadRight(PR) + $": {mobile.Detail}");
                    stringBuilder.AppendLine($"   {nameof(mobile.Address)}".PadRight(PR) + $": {mobile.Address}");
                    stringBuilder.AppendLine($"   {nameof(mobile.NameSpacedClassName)}".PadRight(PR) + $": {mobile.NameSpacedClassName}{Environment.NewLine}");
                    stringBuilder.AppendLine($"{nameof(Exception)} {nameof(Exception.Message)}(s):");
                    stringBuilder.AppendLine($"{exception}{Environment.NewLine}");
                    throw new ArgumentException(stringBuilder.ToString());
                }
            return instrumentDrivers;
        }

        private static Dictionary<String, Object> GetStationaryTestPlanDefinition() {
            Dictionary<String, Object> instrumentDrivers = GetInstrumentDriversTestExecDefinition();
            foreach (Stationary stationary in testPlanDefinition.InstrumentsTestPlan.Stationary) if (!instrumentDrivers.ContainsKey(stationary.ID)) instrumentDrivers.Remove(stationary.ID);
            return instrumentDrivers;
        }

        public static String ConvertWindowsPathToUrl(String path) {
            String url = path.Replace(@"\", "/");
            if (url.StartsWith("//")) url = "file:" + url;
            if (!url.StartsWith("file://")) url = "file://" + url;
            return url;
        }

        public static void OpenApp(String appPath, String arguments = "") {
            if (File.Exists(appPath)) {
                ProcessStartInfo psi = new ProcessStartInfo {
                    FileName = $"\"{appPath}\"",
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = "",
                    Arguments = String.Equals(arguments, "") ? "" : $"\"{arguments}\""
                    // Paths with embedded spaces require enclosing double-quotes (").
                    // https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
                };
                _ = Process.Start(psi);
            } else InvalidPathError(appPath);
        }

        public static void OpenFolder(String folderPath) {
            if (Directory.Exists(folderPath)) {
                ProcessStartInfo psi = new ProcessStartInfo {
                    FileName = "explorer.exe",
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = $"\"{folderPath}\""
                    // Paths with embedded spaces require enclosing double-quotes (").
                    // Even then, simpler 'System.Diagnostics.Process.Start("explorer.exe", path);' invocation fails - thus using ProcessStartInfo class.
                    // https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
                };
                _ = Process.Start(psi);
            } else InvalidPathError(folderPath);
        }

        public static Boolean RegexInvalid(String RegularExpression) {
            if (String.IsNullOrWhiteSpace(RegularExpression)) return true;
            try {
                _ = Regex.Match("", RegularExpression);
            } catch (ArgumentException) {
                return true;
            }
            return false;
        }

        public static void SendDevelopersMailMessage(String Subject, Exception Ex) {
            const Int32 PR = 22;
            StringBuilder stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"{nameof(Environment.MachineName)}".PadRight(PR) + $": {Environment.MachineName}");
            _ = stringBuilder.AppendLine($"{UserName}".PadRight(PR) + $": {UserName}");
            _ = stringBuilder.AppendLine($"Exception.ToString()".PadRight(PR) + $": {Ex}");
            SendDevelopersMailMessage(Subject, Body: stringBuilder.ToString());
        }

        public static void SendDevelopersMailMessage(String Subject, String Body) {
            try {
                Outlook.MailItem mailItem = GetMailItem();
                mailItem.Subject = Subject;
                mailItem.To = testPlanDefinition.Development.EMailAddresses;
                mailItem.Importance = Outlook.OlImportance.olImportanceHigh;
                mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
                mailItem.Body = Body;
                mailItem.Send();
            } catch {
                _ = MessageBox.Show($"Sorry, cannot E-Mail presently.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        public static Outlook.MailItem GetMailItem() {
            Outlook.Application outlook;
            if (Process.GetProcessesByName("OUTLOOK").Length > 0) {
                outlook = Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
            } else {
                outlook = new Outlook.Application();
                Outlook.NameSpace nameSpace = outlook.GetNamespace("MAPI");
                nameSpace.Logon("", "", true, true);
                nameSpace = null;
            }
            return outlook.CreateItem(Outlook.OlItemType.olMailItem);
        }

        public static async Task LoadDeveloperAddresses() {
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook.NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");
            Outlook.AddressList addressList = outlookNamespace.AddressLists["Offline Global Address List"];

            if (addressList != null) {
                try {
                    Object task;
                    foreach (Developer developer in testPlanDefinition.Development.Developer) {
                        task = await Task.Run(() => GetAddress(addressList, developer.Name));
                        developer.EMailAddress = (String)task;
                        if (!String.Equals(developer.EMailAddress, String.Empty)) testPlanDefinition.Development.EMailAddresses += $"{developer.EMailAddress}; ";
                    }
                } catch { }
                if (testPlanDefinition.Development.EMailAddresses.EndsWith("; ")) testPlanDefinition.Development.EMailAddresses = testPlanDefinition.Development.EMailAddresses.Substring(0, testPlanDefinition.Development.EMailAddresses.Length - 2);
            }
        }

        private static String GetAddress(Outlook.AddressList addressList, String Name) {
            Outlook.ExchangeUser exchangeUser;
            foreach (Outlook.AddressEntry entry in addressList.AddressEntries) {
                if (entry != null) {
                    exchangeUser = entry.GetExchangeUser();
                    if (exchangeUser != null && String.Equals(exchangeUser.Name, Name)) return exchangeUser.PrimarySmtpAddress;
                }
            }
            return String.Empty;
        }

        public static void ErrorMessage(String Error) {
            _ = MessageBox.Show($"Unexpected error:{Environment.NewLine}{Error}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        public static void ErrorMessage(Exception Ex) {
            if (testPlanDefinition != null && testPlanDefinition.Development.Developer != null && testPlanDefinition.Development.EMailAddresses != null) {
                if (!String.Equals(testPlanDefinition.Development.EMailAddresses, String.Empty)) {
                    ErrorMessage($"'{Ex.Message}'{Environment.NewLine}{Environment.NewLine}Will attempt to E-Mail details To {testPlanDefinition.Development.EMailAddresses}.{Environment.NewLine}{Environment.NewLine}Please select your Microsoft 365 Outlook profile if dialog appears.");
                    SendDevelopersMailMessage("Exception caught!", Ex);
                }
            }
        }

        public static String NotImplementedMessageEnum(Type enumType) { return $"Unimplemented Enum item; switch/case must support all items in enum '{String.Join(",", Enum.GetNames(enumType))}'."; }

        private static void InvalidPathError(String InvalidPath) { _ = MessageBox.Show($"Path {InvalidPath} invalid.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly); }
    }
}
