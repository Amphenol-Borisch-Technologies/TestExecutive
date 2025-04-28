using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ABT.Test.TestExecutive.TestLib;

namespace ABT.Test.TestExecutive.TestChooser {
    public static class TestChooser {
        [STAThread]
        public static void Main() {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = Data.TestPlansFolder;
                openFileDialog.Filter = "TestPlan Files|*.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK) _ = Process.Start($"\"{openFileDialog.FileName}\"");
            }
        }

        public static void Launch(String NewTestPlanPath, Int32 CurrentTestPlanProcessID) {
            if (!File.Exists(NewTestPlanPath)) {
                _ = MessageBox.Show($"TestPlan '{NewTestPlanPath}' not found.{Environment.NewLine}{Environment.NewLine}Please contact Test Engineering.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            Process testPlanCurrent = null;
            try {
                Cursor.Current = Cursors.WaitCursor;
                testPlanCurrent = Process.GetProcessById(CurrentTestPlanProcessID);
                Int32 iterations = 0, iterationsMax = 60;
                while (!testPlanCurrent.HasExited && iterations <= iterationsMax) {
                    Thread.Sleep(500);
                    testPlanCurrent.Refresh();
                    iterations++; // 60 iterations with 0.5 second sleeps = 30 seconds max.
                }
            } finally {
                Cursor.Current = Cursors.Default;
            }
            if (testPlanCurrent != null && !testPlanCurrent.HasExited) {
                _ = MessageBox.Show($"Current TestPlan hasn't exited.{Environment.NewLine}{Environment.NewLine}Please contact Test Engineering.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            _ = Process.Start($"\"{NewTestPlanPath}\"");
        }
    }
}
