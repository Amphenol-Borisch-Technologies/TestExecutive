using ABT.Test.TestLib.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static ABT.Test.TestLib.Data;

namespace TestDev {
    public partial class TestDev : Form {
        public TestDev() { InitializeComponent(); }

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
