using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.Configuration {
    public partial class TestSelect : Form {
        private static TestSequence testSequence = new TestSequence();

        public TestSelect() {
            InitializeComponent();
            TestList.MultiSelect = false;
            TestOperations.Enabled = TestOperations.Checked = TestGroups.Enabled = true;
            TestGroups.Checked = false;
            ListLoad();
        }

        public static TestSequence Get() {
            TestSelect testSelect = new TestSelect();
            testSelect.ShowDialog(); // Waits until user clicks OK button.
            testSelect.Dispose();
            return testSequence;
        }

        private void ListLoad() {
            TestList.Clear();
            TestList.View = View.Details;
            TestList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            if (TestOperations.Checked) {
                TestGroups.Checked = false;
                Text = $"Select {nameof(TestOperation)}";
                TestList.Columns.Add(nameof(TestOperation));
                TestList.Columns.Add(nameof(TestGroup));
                foreach (TestOperation testOperation in TestLib.testPlanDefinition.TestSpace.TestOperations)
                    if (testOperation.ProductionTest) TestList.Items.Add(new ListViewItem(new String[] { testOperation.NamespaceTrunk, testOperation.Description }));
            } else {
                TestOperations.Checked = false;
                Text = $"Select {nameof(TestGroup)}";
                TestList.Columns.Add(nameof(TestOperation));
                TestList.Columns.Add(nameof(TestGroup));
                TestList.Columns.Add(nameof(TestGroup.Description));
                foreach (TestOperation testOperation in TestLib.testPlanDefinition.TestSpace.TestOperations) {
                    foreach (TestGroup testGroup in testOperation.TestGroups)
                        if (testGroup.Independent) TestList.Items.Add(new ListViewItem(new String[] { testOperation.NamespaceTrunk, testGroup.Classname, testGroup.Description }));
                }
            }
            foreach (ColumnHeader ch in TestList.Columns) ch.Width = -2;
            TestList.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            if (TestList.Columns[0].Width < 115) TestList.Columns[0].Width = 115;
            TestList.ResetText();
            OK.Enabled = false;
        }

        private void TestList_Changed(Object sender, ListViewItemSelectionChangedEventArgs e) { OK.Enabled = (TestList.SelectedItems.Count == 1); }

        private void TestList_MouseDoubleClick(Object sender, MouseEventArgs e) { OK_Click(sender, e); }

        private void OK_Click(Object sender, EventArgs e) {
            Debug.Assert(TestList.SelectedItems.Count == 1);

            testSequence.IsOperation = TestOperations.Checked;
            TestOperation selectedOperation = TestLib.testPlanDefinition.TestSpace.TestOperations.Find(TO => TO.NamespaceTrunk.Equals(TestList.SelectedItems[0].SubItems[0].Text));
            testSequence.TestOperation = Serializing.DeserializeFromFile<TestOperation>(xmlFile: TestLib.TestPlanDefinitionXML_Path, xPath: $"//TestOperation[@NamespaceTrunk='{selectedOperation.NamespaceTrunk}']");
            if (!testSequence.IsOperation) {
                TestGroup selectedGroup = selectedOperation.TestGroups.Find(TG => TG.Classname.Equals(TestList.SelectedItems[0].SubItems[1].Text));
                _ = testSequence.TestOperation.TestGroups.RemoveAll(TG => TG.Classname != selectedGroup.Classname);
                // From the selected TestOperation, retain only the selected TestGroup and all its Methods.
            }
            TestLib.PaddingRight = 0;
            foreach (TestGroup testGroup in testSequence.TestOperation.TestGroups)
                foreach (Method method in testGroup.Methods)
                    if (method.Name.Length > TestLib.PaddingRight) TestLib.PaddingRight = method.Name.Length;
            TestLib.PaddingRight += TestLib.Spaces4.Length;
            DialogResult = DialogResult.OK;
        }

        private void TestOperations_Clicked(Object sender, EventArgs e) { ListLoad(); }

        private void TestGroups_Clicked(Object sender, EventArgs e) { ListLoad(); }
    }
}
