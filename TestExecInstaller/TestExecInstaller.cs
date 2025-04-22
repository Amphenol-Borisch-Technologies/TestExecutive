using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using ABT.Test.TestLibrary.TestLib;

namespace ABT.Test.TestExecutive.TestExecInstaller {
    [RunInstaller(true)]
    public partial class TestExecInstaller : Installer {
        public TestExecInstaller() { InitializeComponent(); }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver) {
            base.Install(stateSaver);
            SetDirectoryPermissions(Data.TEST_EXECUTIVE_DATA, WellKnownSidType.BuiltinUsersSid, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(Data.TEST_EXECUTIVE_PROGRAM, WellKnownSidType.BuiltinUsersSid, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(Data.TEST_EXECUTIVE_DATA, Data.TEST_EXECUTIVE_ADMINISTRATORS, FileSystemRights.Modify | FileSystemRights.Write); // FileSystemRights.Modify includes FileSystemRights.ReadAndExecute.
            SetDirectoryPermissions(Data.TEST_EXECUTIVE_PROGRAM, Data.TEST_EXECUTIVE_ADMINISTRATORS, FileSystemRights.Modify | FileSystemRights.Write);
        }
        private void SetDirectoryPermissions(String directory, WellKnownSidType wellKnownSidType, FileSystemRights fileSystemRights) {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule(
                    new SecurityIdentifier(wellKnownSidType, null),
                        fileSystemRights,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
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
