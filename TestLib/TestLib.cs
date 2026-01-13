using ABT.Test.TestExecutive.TestLib.Configuration;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

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
        // - When I confronted CoPilot with this info, it agreed that flag value overrides definition order, and iterations occur in flag value order, lowest to highest.
        EMERGENCY_STOP = 0b0000_0001, // Most critical event.
        ERROR = 0b0000_0010,          // Second most critical event.
        CANCEL = 0b0000_0100,         // Third most critical event.
        FAIL = 0b0000_1000,           //   .
        UNSET = 0b0001_0000,          //   .
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
        public static readonly String xml = ".xml", xsd = ".xsd";
        public static readonly String TestExecutiveFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly String TestExecDefinitionBase = "TestExecDefinition";
        public static readonly String TestExecDefinitionXML_Path = TestExecutiveFolder + @"\" + TestExecDefinitionBase + xml;
        public static readonly String TestExecDefinitionXSD_Path = TestExecutiveFolder + @"\" + TestExecDefinitionBase + xsd;
        public static readonly TestExecDefinition testExecDefinition = Serializing.DeserializeFromFile<TestExecDefinition>(TestExecDefinitionXML_Path);
        public static readonly String TestExecDefinitionXSD_URL = testExecDefinition.TestExecutiveURL + "/" + TestExecDefinitionBase + xsd;

        public static readonly String TestPlanDefinitionBase = "TestPlanDefinition";
        public static String TestPlanDefinitionXML_Path { get; set; } = null;
        public static readonly String TestPlanDefinitionXSD_Path = TestExecutiveFolder + @"\" + TestPlanDefinitionBase + xsd;
        public static readonly String TestPlanDefinitionXSD_URL = testExecDefinition.TestExecutiveURL + "/" + TestPlanDefinitionBase + xsd;

        public static readonly String TestSequenceBase = "TestSequence";
        public static readonly String TestSequenceXSD_Path = TestExecutiveFolder + @"\" + TestSequenceBase + xsd;
        public static readonly String TestSequenceXSD_URL = testExecDefinition.TestExecutiveURL + "/" + TestSequenceBase + xsd;

        public static readonly String Spaces2 = "  ";
        public static readonly Int32 PaddingRight = 21;
        internal static readonly String TestExecutive = nameof(TestExecutive);

        public static TestPlanDefinition testPlanDefinition { get; set; } = null;
        public static Dictionary<String, Object> InstrumentDrivers { get; set; } = null;
        public static TestSequence testSequence { get; set; } = null;
        public static String UserName { get; set; } = null;
        public static CancellationTokenSource CTS_Cancel { get; set; } = null;
        public static CancellationTokenSource CTS_EmergencyStop { get; set; } = null;
        public static CancellationToken CT_Cancel { get; set; }
        public static CancellationToken CT_EmergencyStop { get; set; }

        public static String FormatMessage(String Label, String Message) { return $"{Spaces2}{Label}".PadRight(PaddingRight) + $": {Message}"; }

        public static Dictionary<String, Object> GetInstrumentDriversTestExecDefinition() {
            Dictionary<String, Object> instrumentDrivers = new Dictionary<String, Object>();

            Object instrumentDriver = null;
            foreach (InstrumentTestExec instrumentTestExec in testExecDefinition.InstrumentsTestExec.InstrumentTestExec) {
                try {
                    if (!testPlanDefinition.TestSpace.Simulate) instrumentDriver = Activator.CreateInstance(Type.GetType(instrumentTestExec.NameSpacedClassName), new Object[] { instrumentTestExec.Address, instrumentTestExec.Detail });
                    instrumentDrivers.Add(instrumentTestExec.ID, instrumentDriver); // instrumentDriver is null if testPlanDefinition.TestSpace.Simulate.
                } catch (Exception exception) {
                    throw new ArgumentException(instrumentTestExec.FormatException(exception));
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
                    throw new ArgumentException(mobile.FormatException(exception));
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

        public static void ErrorMessage(String Error) {
            _ = MessageBox.Show($"Unexpected error:{Environment.NewLine}{Error}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            using (EventLog eventLog = new EventLog(testExecDefinition.WindowsEventLog.Log)) {
                eventLog.Source = testExecDefinition.WindowsEventLog.Source;
                eventLog.WriteEntry(Error, EventLogEntryType.Error);
            }
        }

        public static String NotImplementedMessageEnum<T>(String enumName) where T : Enum { return $"Unimplemented Enum item '{enumName}'; switch/case must support all items in enum '{String.Join(",", Enum.GetNames(typeof(T)))}'."; }

        public static void CopyFolderAndContentsRecursively(String sourceFolder, String destinationFolder) {
            Directory.CreateDirectory(destinationFolder);
            foreach (String filePath in Directory.GetFiles(sourceFolder)) File.Copy(filePath, Path.Combine(destinationFolder, Path.GetFileName(filePath)), overwrite: true);
            foreach (String folderPath in Directory.GetDirectories(sourceFolder)) CopyFolderAndContentsRecursively(folderPath, Path.Combine(destinationFolder, Path.GetFileName(folderPath)));
        }

        public static void TestPlanInstallerCustomActions(String TestPlanInstallationDirectory) {
            // TestExecDefinition actions.
            // NOTE: Use XDocument.Load instead of de-serialized TestLib.testExecDefinition to access TestExecDefinition.xml's Elements and Attributes, because serialization is problematic from within Microsoft Installer.
            XElement testExecDefinition = XDocument.Load(TestExecDefinitionXML_Path).Root;
            String readAndExecute = testExecDefinition.Element(nameof(TestExecDefinition.ActiveDirectoryPermissions)).Attribute(nameof(TestExecDefinition.ActiveDirectoryPermissions.ReadAndExecute)).Value;
            String modifyWrite = testExecDefinition.Element(nameof(TestExecDefinition.ActiveDirectoryPermissions)).Attribute(nameof(TestExecDefinition.ActiveDirectoryPermissions.ModifyWrite)).Value;
            String fullControl = testExecDefinition.Element(nameof(TestExecDefinition.ActiveDirectoryPermissions)).Attribute(nameof(TestExecDefinition.ActiveDirectoryPermissions.ReadAndExecute)).Value;
            String testPlansWorkFolderBase = testExecDefinition.Element(nameof(TestExecDefinition.TestPlansWorkFolderBase)).Value;
            SetDirectoryPermissions(TestPlanInstallationDirectory, readAndExecute, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(TestPlanInstallationDirectory, fullControl, FileSystemRights.FullControl);

            // TestPlanDefinition actions.
            // NOTE: Reading TestExecDefinition.xml is risky, as it's not guaranteed to be present before a TestPlan installation is completed.  Thus far, is working.
            XElement testPlanDefinition = XDocument.Load($@"{TestPlanInstallationDirectory}\{TestPlanDefinitionBase}{xml}").Root;
            XElement testSpace = testPlanDefinition.Element(nameof(TestPlanDefinition.TestSpace));
            XElement uut = testPlanDefinition.Element(nameof(TestPlanDefinition.UUT));
            String number = uut.Attribute(nameof(TestPlanDefinition.UUT.Number)).Value;

            // WorkFolder sub-folders & permissions.
            // NOTE: Assign Modify & Write permissions to TestExecDefinition.xml's ReadAndExecute Group because the ReadAndExecute Group is defined only for permissions to execute the TestExec application.
            // The WorkFolder permissions need to be ModifyWrite because TestPlans create folders & files in their WorkFolders during TestPlan execution.
            CreateDirectoryAndSetPermissions($@"{testPlansWorkFolderBase}\{number}", readAndExecute, FileSystemRights.Modify | FileSystemRights.Write);
            foreach (XElement testOperation in testSpace.Elements(nameof(TestOperation)))
                CreateDirectoryAndSetPermissions($@"{testPlansWorkFolderBase}\{number}\{testOperation.Attribute(nameof(TestOperation.NamespaceTrunk)).Value}", readAndExecute, FileSystemRights.Modify | FileSystemRights.Write);

            // TDR folders, sub-folders & permissions.
            if (testExecDefinition.Element(nameof(TestExecDefinition.TestData))?.Element(nameof(Files)) != null) {
                XElement files = testExecDefinition.Element(nameof(TestExecDefinition.TestData)).Element(nameof(Files));
                CreateDirectoryAndSetPermissions($@"{files.Attribute(nameof(Files.Folder)).Value}\{number}", modifyWrite, FileSystemRights.Modify | FileSystemRights.Write);
                foreach (XElement testOperation in testSpace.Elements(nameof(TestOperation)))
                    if (testOperation.Attribute(nameof(TestOperation.ProductionTest)).Value == "true") CreateDirectoryAndSetPermissions($@"{files.Attribute(nameof(Files.Folder)).Value}\{number}\{testOperation.Attribute(nameof(TestOperation.NamespaceTrunk)).Value}", modifyWrite, FileSystemRights.Modify | FileSystemRights.Write);
            }
        }

        public static void SetDirectoryPermissions(String directory, String identity, FileSystemRights fileSystemRights) {
            try {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                directorySecurity.SetAccessRuleProtection(false, false);
                directorySecurity.AddAccessRule(
                    new FileSystemAccessRule(identity,
                        fileSystemRights,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow));
                directoryInfo.SetAccessControl(directorySecurity);
            } catch (Exception exception) {
                _ = MessageBox.Show(
                    $"Directory:   '{directory}'.{Environment.NewLine}" +
                    $"Identity:     '{identity}'.{Environment.NewLine}" +
                    $"File Rights: '{fileSystemRights}'.{Environment.NewLine}{Environment.NewLine}" +
                    $"{exception.Message}",
                    $"Error Setting Directory Permissions", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void CreateDirectoryAndSetPermissions(String directory, String identity, FileSystemRights fileSystemRights) {
            Directory.CreateDirectory(directory);
            SetDirectoryPermissions(directory, identity, fileSystemRights);
        }

        private static void InvalidPathError(String InvalidPath) { _ = MessageBox.Show($"Path {InvalidPath} invalid.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly); }
    }
}
