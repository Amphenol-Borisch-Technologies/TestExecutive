using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
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

            XElement textFiles = testExecDefinition.Element("TestData").Element("TextFiles");
            if (textFiles != null) {
                SetDirectoryPermissions(textFiles.Attribute("Folder").Value, activeDirectoryPermissions.Attribute("ReadAndExecute").Value, FileSystemRights.ReadAndExecute);
                SetDirectoryPermissions(textFiles.Attribute("Folder").Value, activeDirectoryPermissions.Attribute("ModifyWrite").Value, FileSystemRights.Modify | FileSystemRights.Write);
                SetDirectoryPermissions(textFiles.Attribute("Folder").Value, activeDirectoryPermissions.Attribute("FullControl").Value, FileSystemRights.FullControl);
            }

            if (!EventLog.SourceExists(testExecDefinition.Element("EventSource").Value)) {
                EventLog.CreateEventSource(testExecDefinition.Element("EventSource").Value, "Application");
                EventLog eventLog = new EventLog("Application") {
                    Source = testExecDefinition.Element("EventSource").Value
                };
                eventLog.WriteEntry("First entry.", EventLogEntryType.Information, 1);
            }
        }

        private void SetDirectoryPermissions(String directory, String identity, FileSystemRights fileSystemRights) {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule(identity,
                    fileSystemRights,
                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                    PropagationFlags.NoPropagateInherit,
                    AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState) { base.Commit(savedState); }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState) { base.Rollback(savedState); }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState) { base.Uninstall(savedState); }
    }
}
