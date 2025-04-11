using ABT.Test.TestExecutive.TestLib.Configuration;
using ABT.Test.TestExecutive.TestLib.Miscellaneous;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using static ABT.Test.TestExecutive.TestLib.Data;

namespace TestDev {
    public partial class TestDev : Form {
        public TestDev() { InitializeComponent(); }
        private void TSMI_BarcodeScanner_Click(Object sender, EventArgs e) {
            DeviceInformationCollection deviceInformationCollectionic = await DeviceInformation.FindAllAsync(BarcodeScanner.GetDeviceSelector(PosConnectionTypes.Local));
            StringBuilder stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"Discovering Microsoft supported, corded Barcode Scanner(s):{Environment.NewLine}");
            _ = stringBuilder.AppendLine($"  - See https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/pos-device-support.");
            _ = stringBuilder.AppendLine($"  - Note that only corded Barcode Scanners are discovered; cordless BlueTooth & Wireless scanners are ignored.");
            _ = stringBuilder.AppendLine($"  - Note that cameras are also discovered; cameras are digital imagers, just as many bar-code readers are.");
            _ = stringBuilder.AppendLine($"  - Modify ConfigurationTestExec to use a discovered Barcode Scanner.");
            _ = stringBuilder.AppendLine($"  - Scanners must be programmed into USB-HID mode to function properly:");
            _ = stringBuilder.AppendLine(@"    - See: file:///P:/Test/Engineers/Equipment_Manuals/Honeywell/Honeywell_Voyager_1200G_User's_Guide_ReadMe.pdf");
            _ = stringBuilder.AppendLine($"    - Or:  https://prod-edam.honeywell.com/content/dam/honeywell-edam/sps/ppr/en-us/public/products/barcode-scanners/general-purpose-handheld/1200g/documents/sps-ppr-vg1200-ug.pdf{Environment.NewLine}{Environment.NewLine}");
            foreach (DeviceInformation deviceInformation in deviceInformationCollectionic) {
                _ = stringBuilder.AppendLine($"Name: '{deviceInformation.Name}'.");
                _ = stringBuilder.AppendLine($"Kind: '{deviceInformation.Kind}'.");
                _ = stringBuilder.AppendLine($"ID  : '{deviceInformation.Id}'.{Environment.NewLine}");
            }

            CustomMessageBox.Show(Title: $"Microsoft supported, corded Barcode Scanner(s)", Message: stringBuilder.ToString());
        }

        private void TSMI_TestDefinitions_TestChooser_Click(Object sender, EventArgs e) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(GetTestLibExecutionDirectory(), "TestChooser Definition File|TestChooserDefinition.xml");
            if (dialogResult == DialogResult.OK) OpenApp(fileName);
        }
        private void TSMI_TestDefinitions_TestExec_Click(Object sender, EventArgs e) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(GetTestLibExecutionDirectory(), "TestExec Definition File|TestExecDefinition.xml");
            if (dialogResult == DialogResult.OK) OpenApp(fileName);
        }
        private void TSMI_TestDefinitions_TestPlans_Click(Object sender, EventArgs e) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(GetTestPlansFolder(), "TestPlan Definition File|TestPlanDefinition.xml");
            if (dialogResult == DialogResult.OK) OpenApp(fileName);
        }
        private void TSMI_TestDefinitions_Validate_Click(Object sender, EventArgs e) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(GetTestPlansFolder(), "TestPlan Definition File|TestPlanDefinition.xml");
            if (dialogResult == DialogResult.OK) {
                if (TestPlanDefinitionValidator.ValidSpecification(GetTestLibExecutionDirectory() + @"\TestPlanDefinition.xsd", fileName)) _ = MessageBox.Show(this, "Validation passed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TSMI_Generate_Project_Click(Object sender, EventArgs e) { }
        private void TSMI_Generate_InstrumentAliases_Click(Object sender, EventArgs e) { TestPlanDefinitionAction(TestPlanGenerator.GenerateInstrumentAliases); }
        private void TSMI_Generate_TestPlan_Click(Object sender, EventArgs e) { TestPlanDefinitionAction(TestPlanGenerator.GenerateTestPlan); }
        private void TestPlanDefinitionAction(Action<String> executeAction) {
            (DialogResult dialogResult, String fileName) = GetTestDefinitionFile(GetTestPlansFolder(), "TestPlan Definition File|TestPlanDefinition.xml");
            if (dialogResult == DialogResult.OK) {
                if (!TestPlanDefinitionValidator.ValidSpecification(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\TestPlanDefinition.xsd", fileName)) return;
                executeAction?.Invoke(fileName);
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

        private void TSMI_Keysight_CommandExpert_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Keysight.CommandExpert); }
        private void TSMI_Keysight_ConnectionExpert_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Keysight.ConnectionExpert); }

        private void TSMI_Microsoft_SQL_ServerManagementStudio_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.SQLServerManagementStudio); }
        private void TSMI_Microsoft_VisualStudio_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.VisualStudio); }
        private void TSMI_Microsoft_VisualStudioCode_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.VisualStudioCode); }
        private void TSMI_Microsoft_XML_Notepad_Click(Object sender, EventArgs e) { OpenApp(testExecDefinition.Apps.Microsoft.XMLNotepad); }

        private void TSMI_TestPlans_Choose_Click(Object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = GetTestPlansFolder();
                openFileDialog.Filter = GetTestPlansFilter();
                if (openFileDialog.ShowDialog() == DialogResult.OK) _ = Process.Start($"\"{openFileDialog.FileName}\"");
            }
        }
    }
}
