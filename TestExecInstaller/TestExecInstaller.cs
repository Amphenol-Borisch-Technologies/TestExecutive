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
        // Consider the below C:\Windows\System32\MSIexec.exe error:
        //   === Logging started: 4/15/2025  10:47:52 ===
        //   Error 1001. Error 1001. Exception occurred while initializing the installation:
        //   System.BadImageFormatException: Could not load file or assembly 'file:///C:\Program Files\ABT\Test\TestExecutive\TestInstaller.dll' or one of its dependencies.
        //   An attempt was made to load a program with an incorrect format..
        //
        // This C:\Windows\System32\MSIexec.exe error occurs:
        // - If TestExecraryInstaller is compiled as a 64-bit assembly, with the rest of
        //   TestExecrary compiled as 64-bit and the target machine also 64-bit.
        // - Compiling TestExecraryInstaller as a 32-bit assembly, with the rest of
        //   TestExecrary compiled as 64-bit and the target machine also 64-bit resolves this error.
        // - C:\Windows\System32\MSIexec.exe is the 64-bit version of the Windows Installer:
        //   - Speculate C:\Windows\System32\MSIexec.exe potentially invokes a 32 bit shim DLL somewhere, causing this error.
        //
        // Unfortunately, compiling TestExecraryInstaller as a 32-bit assembly causes the following MSBuild Warning:
        //   2>C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\amd64\Microsoft.Common.CurrentVersion.targets(2424,5):
        //   warning MSB3270: There was a mismatch between the processor architecture of the project being built "x86"
        //   and the processor architecture of the reference "C:\Users\phils\source\repos\ABT\Test\TestExecrary\TestExec\bin\x64\TestExecrary\TestExec.dll", "AMD64".
        //   This mismatch may cause runtime failures.
        //   Please consider changing the targeted processor architecture of your project through the Configuration Manager so as to align the processor architectures between your project and references,
        //   or take a dependency on references with a processor architecture that matches the targeted processor architecture of your project.
        // - But following this advice restores the original C:\Windows\System32\MSIexec.exe Error 1001.
        // - Ignoring this warning still allows the installer to run successfully.

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
