using System;
using System.Collections;
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

            XElement activeDirectoryPermissions = testExecDefinition.Element("ActiveDirectoryPermissions");
            SetDirectoryPermissions(Context.Parameters["targetdir"], activeDirectoryPermissions.Attribute("ReadAndExecute").Value, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(Context.Parameters["targetdir"], activeDirectoryPermissions.Attribute("FullControl").Value, FileSystemRights.FullControl);

            Directory.CreateDirectory(testExecDefinition.Element("TestPlansFolder").Value);
            SetDirectoryPermissions(testExecDefinition.Element("TestPlansFolder").Value, activeDirectoryPermissions.Attribute("ReadAndExecute").Value, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(testExecDefinition.Element("TestPlansFolder").Value, activeDirectoryPermissions.Attribute("FullControl").Value, FileSystemRights.FullControl);

            // NOTE: SetDirectoryPermissions(textFiles.Attribute("Folder").Value) fails, apparently because I can't change permissions on P:\Test\TDR.
            //XElement textFiles = testExecDefinition.Element("TestData").Element("TextFiles");
            //if (textFiles != null) {
            //    SetDirectoryPermissions(textFiles.Attribute("Folder").Value, activeDirectoryPermissions.Attribute("ReadAndExecute").Value, FileSystemRights.ReadAndExecute);
            //    SetDirectoryPermissions(textFiles.Attribute("Folder").Value, activeDirectoryPermissions.Attribute("ModifyWrite").Value, FileSystemRights.Modify | FileSystemRights.Write);
            //    SetDirectoryPermissions(textFiles.Attribute("Folder").Value, activeDirectoryPermissions.Attribute("FullControl").Value, FileSystemRights.FullControl);
            //}

            String source = testExecDefinition.Element("WindowsEventLog").Attribute("Source").Value;
            String log = testExecDefinition.Element("WindowsEventLog").Attribute("Log").Value;
            try {
                if (!EventLog.SourceExists(source)) {
                    EventLog.CreateEventSource(source, log);
                    Int32 i = 5; while (i-- > 0 && !EventLog.Exists(log)) Thread.Sleep(1000);
                    if (EventLog.Exists(log)) using (EventLog eventLog = new EventLog() { Source = source }) { eventLog.WriteEntry("Created today.", EventLogEntryType.Information, 0); }
                } else {
                    if (EventLog.Exists(log)) using (EventLog eventLog = new EventLog() { Source = source }) { eventLog.WriteEntry("Created previously.", EventLogEntryType.Information, 1); }
                }
            } catch (Exception exception) {
                _ = MessageBox.Show(
                    $"Source: '{source}'.{Environment.NewLine}" +
                    $"Log:    '{log}'.{Environment.NewLine}{Environment.NewLine}" +
                    $"{exception.Message}",
                    $"Error Creating or Writing Event Log & Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
