using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using ABT.Test.TestLibrary.TestLib;

namespace ABT.Test.TestExecutive.TestChooser {
    public static class TestChooser {
        [STAThread]
        public static void Main(String[] args) {
            // NOTE: Below MSBuild warning is expected & desirable:
            // WARNING: Configuration file 'C:\Users\phils\source\repos\ABT\Test\TestExecutive\TestChooser\bin\x64\Release\TestChooser.exe.config' is being used to configure all executables
            // - Above means all executables in the TestExecutive solution are using the same configuration file, as desired.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Int32 testPlanOldPID = 0;
            if (args.Length > 0) try { testPlanOldPID = Convert.ToInt32(args[0]); } catch { }
            if (testPlanOldPID != 0) {
                Process testPlanOld = null;
                try {
                    Cursor.Current = Cursors.WaitCursor;
                    testPlanOld = Process.GetProcessById(testPlanOldPID);
                    Int32 iterations = 0, iterationsMax = 60;
                    while (!testPlanOld.HasExited && iterations <= iterationsMax) {
                        Thread.Sleep(500);
                        testPlanOld.Refresh();
                        iterations++; // 60 iterations with 0.5 second sleeps = 30 seconds max.
                    }
                } finally {
                    Cursor.Current = Cursors.Default;
                }
                if (testPlanOld != null && !testPlanOld.HasExited) {
                    _ = MessageBox.Show($"Old TestPlan hasn't exited.{Environment.NewLine}{Environment.NewLine}Please contact Test Engineering.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = Data.TEST_PLANS_PROGRAMS;
                openFileDialog.Filter = "TestPlan Files|*.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK) _ = Process.Start($"\"{openFileDialog.FileName}\"");
            }
        }
    }
}
