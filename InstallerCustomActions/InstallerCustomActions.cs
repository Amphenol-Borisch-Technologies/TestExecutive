using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.InstallerCustomActions {
    [RunInstaller(true)]
    public partial class InstallerCustomActions : Installer {
        public InstallerCustomActions() { InitializeComponent(); }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver) {
            base.Install(stateSaver);
            SetDirectoryPermissions(Context.Parameters["targetdir"], WellKnownSidType.AccountDomainUsersSid, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(Context.Parameters["targetdir"], @"BORISCH\Test - TestExecutive Administrators", FileSystemRights.FullControl);

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            String eventSource = configuration.AppSettings.Settings["EventSource"].Value;
            if (!EventLog.SourceExists(eventSource)) {
                EventLog.CreateEventSource(eventSource, "Application");
                EventLog eventLog = new EventLog("Application") {
                    Source = eventSource
                };
                eventLog.WriteEntry("First entry.", EventLogEntryType.Information, 1);
            }
        }
        private void SetDirectoryPermissions(String directory, WellKnownSidType wellKnownSidType, FileSystemRights fileSystemRights) {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule(
                    new SecurityIdentifier(wellKnownSidType, WindowsIdentity.GetCurrent()?.User?.AccountDomainSid),
                        fileSystemRights,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }

        private SecurityIdentifier GetDomainSid() {
            try {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) { return identity?.User?.AccountDomainSid; }
            } catch (Exception exception) {
                _ = MessageBox.Show($"Error retrieving Current Windows Identity User's Domain SID: {exception.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
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
