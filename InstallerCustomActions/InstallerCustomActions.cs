using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ABT.Test.TestExecutive.InstallerCustomActions {
    [RunInstaller(true)]
    public partial class InstallerCustomActions : Installer {
        public InstallerCustomActions() { InitializeComponent(); }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver) {
            base.Install(stateSaver);
            XElement testExecDefinition = XDocument.Load(Context.Parameters["targetdir"] + @"\TestExecDefinition.xml").Root;
            // NOTE: Cannot access TestLib.TestExecDefinitionXML_Path because it may not be usable yet, as this, it's installation, hasn't yet completed.
            // Reading TestExecDefinition.xml is also risky, as it's not guaranteed to be present before installation is completed.
            // TODO: Resolve this by using the WiX Toolkit or other Installer besides Microsoft's Installer Project to first completely install TestLib, then reference TestLib's static readonly String paths while subsequently installing TestExec.
            // Installers like WiX Toolkit can automatically sequence multiple installations to handle dependencies like this, whereas Microsoft's Installer Project installers cannot.
            // - Downside is WiX has a non-trivial learning curve.
            // Alternatively, could have separate Microsoft Installer Project installers for TestLib & TestExec, and manually install first TestLib, then TestExec.
            // - Downside is must then have two Installer Projects to maintain, which must be installed in correct sequence.
            // Or could define customer Context.Parameters in this Microsoft Project installer & access them from this method.
            // - Downside is must then synchronize TestLib's static readonly String paths with the Context.Parameters, easily forgotten if changed.
            // Currently using hard-coded string constants, which simple & straitforward.
            // - Downside is they can only be changed via rebuilding solution.
            XElement activeDirectoryPermissions = testExecDefinition.Element("ActiveDirectoryPermissions");
            String readAndExecute = activeDirectoryPermissions.Attribute("ReadAndExecute").Value;
            String modifyWrite = activeDirectoryPermissions.Attribute("ModifyWrite").Value;
            String fullControl = activeDirectoryPermissions.Attribute("FullControl").Value;
            SetDirectoryPermissions(Context.Parameters["targetdir"], readAndExecute, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(Context.Parameters["targetdir"], fullControl, FileSystemRights.FullControl);

            String testPlansInstallationFolderBase = testExecDefinition.Element("TestPlansInstallationFolderBase").Value;
            Directory.CreateDirectory(testPlansInstallationFolderBase);
            SetDirectoryPermissions(testPlansInstallationFolderBase, readAndExecute, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(testPlansInstallationFolderBase, fullControl, FileSystemRights.FullControl);

            String testPlansWorkFolderBase = testExecDefinition.Element("TestPlansWorkFolderBase").Value;
            const String programData = @"C:\ProgramData";
            if (IsSubPath(programData, testPlansWorkFolderBase)) {
                // Try to set permissions on each segment of the path from ProgramData to TestPlansWorkFolderBase.
                foreach (String subPath in EnumeratePathSegments(programData, testPlansWorkFolderBase)) {
                    Directory.CreateDirectory(subPath);
                    SetDirectoryPermissions(subPath, readAndExecute, FileSystemRights.Modify | FileSystemRights.Write);
                    // NOTE: Assign Modify & Write permissions to TestExecDefinition.xml's ReadAndExecute Group because the ReadAndExecute Group is defined only for permissions to execute the TestExec application.
                    // The WorkFolder permissions need to be ModifyWrite because TestPlans create folders & files in their WorkFolders during TestPlan execution.
                    SetDirectoryPermissions(subPath, fullControl, FileSystemRights.FullControl);
                }
            } else {
                // Just set permissions on the TestPlansWorkFolderBase folder.
                SetDirectoryPermissions(testPlansWorkFolderBase, readAndExecute, FileSystemRights.Modify | FileSystemRights.Write);
                SetDirectoryPermissions(testPlansWorkFolderBase, fullControl, FileSystemRights.FullControl);
            }

                // NOTE: SetDirectoryPermissions(folder) fails, apparently because I cannot change permissions on P:\Test\TDR.
                //XElement textFiles = testExecDefinition.Element("TestData").Element("Files");
                //if (textFiles != null) {
                //    String folder = textFiles.Attribute("Folder").Value;
                //    String modifyWrite = activeDirectoryPermissions.Attribute("ModifyWrite").Value;
                //    SetDirectoryPermissions(folder, readAndExecute, FileSystemRights.ReadAndExecute);
                //    SetDirectoryPermissions(folder, modifyWrite, FileSystemRights.Modify | FileSystemRights.Write);
                //    SetDirectoryPermissions(folder, fullControl, FileSystemRights.FullControl);
                //}

                String source = testExecDefinition.Element("WindowsEventLog").Attribute("Source").Value;
            String log = testExecDefinition.Element("WindowsEventLog").Attribute("Log").Value;
            try {
                Boolean sourceExisted = EventLog.SourceExists(source);
                if (!sourceExisted) {
                    EventLog.CreateEventSource(source, log);
                    Int32 i = 5; while (i-- > 0 && !EventLog.Exists(log)) Thread.Sleep(1000);
                }
                if (EventLog.Exists(log)) {
                    using (EventLog eventLog = new EventLog() { Source = source }) {
                        if (sourceExisted) eventLog.WriteEntry("Created previously.", EventLogEntryType.Information, 1);
                        else eventLog.WriteEntry("Created today.", EventLogEntryType.Information, 0);
                    }
                }
            } catch (Exception exception) {
                _ = MessageBox.Show(
                    $"Source: '{source}'.{Environment.NewLine}" +
                    $"Log:    '{log}'.{Environment.NewLine}{Environment.NewLine}" +
                    $"{exception.Message}",
                    $"Error Creating or Writing Event Log & Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Boolean IsSubPath(String parentPath, String childPath) {
            var parentUri = new Uri(AppendDirectorySeparator(parentPath), UriKind.Absolute);
            var childUri = new Uri(AppendDirectorySeparator(childPath), UriKind.Absolute);
            return parentUri.IsBaseOf(childUri);
        }

        private String AppendDirectorySeparator(String path) {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString())) return path + Path.DirectorySeparatorChar;
            return path;
        }

        private IEnumerable<String> EnumeratePathSegments(String parentPath, String childPath) {
            String parent = Path.GetFullPath(parentPath).TrimEnd(Path.DirectorySeparatorChar);
            String current = Path.GetFullPath(childPath).TrimEnd(Path.DirectorySeparatorChar);
            if (!current.StartsWith(parent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) && !current.Equals(parent, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("childPath is not under parentPath.");

            List<String> list = new List<String>();
            while (!current.Equals(parent, StringComparison.OrdinalIgnoreCase)) {
                list.Add(current);
                current = Directory.GetParent(current).FullName;
            }
            list.Reverse();
            return list;
        }

        private void SetDirectoryPermissions(String directory, String identity, FileSystemRights fileSystemRights) {
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

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState) { base.Commit(savedState); }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState) { base.Rollback(savedState); }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState) { base.Uninstall(savedState); }
    }
}
