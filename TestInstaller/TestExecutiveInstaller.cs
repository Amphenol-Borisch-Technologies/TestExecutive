using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TestInstaller {
    [RunInstaller(true)]
    public partial class TestExecutiveInstaller : Installer {
        private const String _ABT_TEST = @"\ABT\TEST";
        private const String _BASEPATH_DATA = @"C:\ProgramData" + _ABT_TEST;
        private const String _TEST_EXECUTIVE = @"\TestExecutive";
        public const String TEST_EXECUTIVE_DATA = _BASEPATH_DATA + _TEST_EXECUTIVE;

        public TestExecutiveInstaller() {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver) {
            base.Install(stateSaver);
            if (!Directory.Exists(TEST_EXECUTIVE_DATA)) Directory.CreateDirectory(TEST_EXECUTIVE_DATA);
            DirectoryInfo directoryInfo = new DirectoryInfo(TEST_EXECUTIVE_DATA);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
                        FileSystemRights.Read | FileSystemRights.ReadAndExecute,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow));
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule(@"BORISCH\Test - Engineers",
                        FileSystemRights.Modify | FileSystemRights.Read | FileSystemRights.ReadAndExecute | FileSystemRights.Write,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState) { base.Commit(savedState); }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState) {
            base.Rollback(savedState);
            if (Directory.Exists(TEST_EXECUTIVE_DATA)) Directory.Delete(TEST_EXECUTIVE_DATA, true);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState) {
            //Process application = null;
            //foreach (Process process in Process.GetProcesses()) {
            //    if (!process.ProcessName.ToLower().Contains("yourprocessname")) continue;
            //    application = process;
            //    break;
            //}

            //if (application != null && application.Responding) {
            //    application.Kill();
            //    base.Uninstall(savedState);
            //}
            base.Uninstall(savedState);
            if (Directory.Exists(TEST_EXECUTIVE_DATA)) Directory.Delete(TEST_EXECUTIVE_DATA, true);
        }
    }
}
