using ABT.Test.TestExecutive.TestLib;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TestInstaller {
    [RunInstaller(true)]
    public partial class TestExecutiveInstaller : Installer {
        public TestExecutiveInstaller() {
            InitializeComponent();
        }

        public override void Install(IDictionary savedState) {
            base.Install(savedState);
            if (!Directory.Exists(Data.TEST_EXECUTIVE_DATA)) Directory.CreateDirectory(Data.TEST_EXECUTIVE_DATA);
            DirectoryInfo directoryInfo = new DirectoryInfo(Data.TEST_EXECUTIVE_DATA);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                        FileSystemRights.FullControl,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow));
            directorySecurity.AddAccessRule(new FileSystemAccessRule("Test - Engineers", FileSystemRights.Write, AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }

        public override void Rollback(IDictionary savedState) {
            base.Rollback(savedState);
            if (Directory.Exists(Data.TEST_EXECUTIVE_DATA)) Directory.Delete(Data.TEST_EXECUTIVE_DATA, true);
        }

        public override void Commit(IDictionary savedState) { base.Commit(savedState); }

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
            if (Directory.Exists(Data.TEST_EXECUTIVE_DATA)) Directory.Delete(Data.TEST_EXECUTIVE_DATA, true);
        }
    }
}
