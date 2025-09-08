using ABT.Test.TestExecutive.TestLib.Configuration;
using ABT.Test.TestExecutive.TestLib.Miscellaneous;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestDev {
    public partial class TestDev : Form {
        public TestDev() { InitializeComponent(); }
        private void TSMI_Apps_ABT_TestChooser_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.ABT.TestChooser); }
        private void TSMI_Apps_Keysight_CommandExpert_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Keysight.CommandExpert); }
        private void TSMI_Apps_Keysight_ConnectionExpert_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Keysight.ConnectionExpert); }
        private void TSMI_Apps_Microsoft_EventViewer_Click(object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.EventViewer, $"/c:{testExecDefinition.WindowsEventLog.Log}"); }
        private void TSMI_Apps_Microsoft_SQLServerManagementStudio_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.SQLServerManagementStudio); }
        private void TSMI_Apps_Microsoft_VisualStudio_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.VisualStudio); }
        private void TSMI_Apps_Microsoft_VisualStudioCode_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.VisualStudioCode); }
        private void TSMI_Apps_Microsoft_XMLNotepad_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.XMLNotepad); }

        private async void TSMI_BarcodeScanner_Click(Object sender, EventArgs e) {
            DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(BarcodeScanner.GetDeviceSelector(PosConnectionTypes.Local));
            StringBuilder stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"Discovering Barcode Scanner(s):{Environment.NewLine}");
            _ = stringBuilder.AppendLine($"  - Note that only local, barcode scanning capable devices, explicitly supported by Microsoft's Windows.Devices.PointOfService.BarcodeScanner class are discovered.");
            _ = stringBuilder.AppendLine($"    - Bluetooth & Ethernet/WiFi scanners aren't local and aren't discovered.");
            _ = stringBuilder.AppendLine($"    - USB, serial, parallel & integrated scanners are local, and discovered if Microsoft supported.");
            _ = stringBuilder.AppendLine($"    - Wired & integrated Webcams, being local & scanning capable, are discovered if Microsoft supported.");
            _ = stringBuilder.AppendLine($"    - See https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/pos-device-support.");
            _ = stringBuilder.AppendLine($"  - Modify ConfigurationTestExec to use the desired discovered Barcode Scanner.");
            _ = stringBuilder.AppendLine($"  - Scanners must be programmed into USB-HID mode to function properly:");
            _ = stringBuilder.AppendLine(@"    - See: file:///P:/Test/Engineers/Equipment_Manuals/Honeywell/Honeywell_Voyager_1200G_User's_Guide_ReadMe.pdf");
            _ = stringBuilder.AppendLine($"    - Or:  https://prod-edam.honeywell.com/content/dam/honeywell-edam/sps/ppr/en-us/public/products/barcode-scanners/general-purpose-handheld/1200g/documents/sps-ppr-vg1200-ug.pdf{Environment.NewLine}{Environment.NewLine}");
            foreach (DeviceInformation deviceInformation in deviceInformationCollection) {
                _ = stringBuilder.AppendLine($"Name: '{deviceInformation.Name}'.");
                _ = stringBuilder.AppendLine($"Kind: '{deviceInformation.Kind}'.");
                _ = stringBuilder.AppendLine($"ID  : '{deviceInformation.Id}'.{Environment.NewLine}");
            }

            CustomMessageBox.Show(Title: $"Discovering Barcode Scanner(s)", Message: stringBuilder.ToString());
        }
        private void TSMI_Generate_Project_Click(Object sender, EventArgs e) { }
        private void TSMI_Generate_InstrumentAliases_Click(Object sender, EventArgs e) { TestPlanDefinitionAction(TestPlanGenerator.GenerateInstrumentAliases); }
        private void TSMI_Generate_TDRFolders_Click(object sender, EventArgs e) {
            if (testExecDefinition.TestData.Item is TextFiles textFiles) {
                String[] testPlanDefinitionPaths = Directory.GetFiles(testExecDefinition.TestPlansFolder, TestPlanDefinitionBase + xml, SearchOption.AllDirectories);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"TDR folders in {testExecDefinition.TestPlansFolder} corresponding to TestPlans in {testExecDefinition.TestPlansFolder}.");

                TestPlanDefinition testPlanDefinition;
                foreach (String testPlanDefinitionPath in testPlanDefinitionPaths) {
                    testPlanDefinition = Serializing.DeserializeFromFile<TestPlanDefinition>(testPlanDefinitionPath);
                    CreateDirectoryAndSetPermissions(textFiles.Folder + "\\" + testPlanDefinition.UUT.Number);
                    stringBuilder.AppendLine($"  {textFiles.Folder + "\\" + testPlanDefinition.UUT.Number}");
                    foreach (TestOperation testOperation in testPlanDefinition.TestSpace.TestOperations) {
                        CreateDirectoryAndSetPermissions(textFiles.Folder + "\\" + testPlanDefinition.UUT.Number + "\\" + testOperation.NamespaceTrunk);
                        stringBuilder.AppendLine($"     {textFiles.Folder + "\\" + testPlanDefinition.UUT.Number + "\\" + testOperation.NamespaceTrunk}");
                    }
                }
                CustomMessageBox.Show(Title: $"TestPlan TDR Folders", Message: stringBuilder.ToString());
            }
        }
        private void SetDirectoryPermissions(String directory, String identity, FileSystemRights fileSystemRights) {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(
                new FileSystemAccessRule(identity,
                    fileSystemRights,
                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }
        private void CreateDirectoryAndSetPermissions(String directory) {
            Directory.CreateDirectory(directory);
            SetDirectoryPermissions(directory, testExecDefinition.ActiveDirectoryPermissions.ReadAndExecute, FileSystemRights.ReadAndExecute);
            SetDirectoryPermissions(directory, testExecDefinition.ActiveDirectoryPermissions.ModifyWrite, FileSystemRights.Modify | FileSystemRights.Write);
            SetDirectoryPermissions(directory, testExecDefinition.ActiveDirectoryPermissions.FullControl, FileSystemRights.FullControl);
        }
        private void TSMI_Generate_TestPlan_Click(Object sender, EventArgs e) { TestPlanDefinitionAction(TestPlanGenerator.GenerateTestPlan); }
        private void TestPlanDefinitionAction(Action<String> executeAction) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(testExecDefinition.TestPlansFolder, $"TestPlan Definition File|{TestPlanDefinitionBase}{xml}");
            if (dialogResult == DialogResult.OK) {
                if (!TestPlanDefinitionValidator.ValidDefinition(fileName)) return;
                executeAction?.Invoke(fileName);
            }
        }

        private void TSMI_TestDefinitions_TestExec_Click(Object sender, EventArgs e) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(TestExecutiveFolder, $"TestExec Definition File|{TestExecDefinitionBase}{xml}");
            if (dialogResult == DialogResult.OK) OpenApp(fileName);
        }
        private void TSMI_TestDefinitions_TestPlans_Click(Object sender, EventArgs e) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(testExecDefinition.TestPlansFolder, $"TestPlan Definition File|{TestPlanDefinitionBase}{xml}");
            if (dialogResult == DialogResult.OK) OpenApp(fileName);
        }
        private void TSMI_TestDefinitions_Validate_Click(Object sender, EventArgs e) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(testExecDefinition.TestPlansFolder, $"TestPlan Definition File|{TestPlanDefinitionBase}{xml}");
            if (dialogResult == DialogResult.OK) {
                if (TestPlanDefinitionValidator.ValidDefinition(fileName)) _ = MessageBox.Show(this, "Validation passed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private (DialogResult, String) GetTestDefinitionFile(String InitialDirectory, String Filter) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = InitialDirectory;
                openFileDialog.Filter = Filter;
                if (openFileDialog.ShowDialog() == DialogResult.OK) return (DialogResult.OK, openFileDialog.FileName);
            }
            return (DialogResult.Cancel, null);
        }

        private void TestDev_Load(object sender, EventArgs e) { TSMI_Generate_TDRFolders.Enabled = (testExecDefinition.TestData.Item is TextFiles textFiles); }
    }
}
