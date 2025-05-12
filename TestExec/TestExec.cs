using ABT.Test.TestExecutive.TestLib;
using ABT.Test.TestExecutive.TestLib.Configuration;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using ABT.Test.TestExecutive.TestLib.Miscellaneous;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using static ABT.Test.TestExecutive.TestLib.TestLib;

// TODO:  Eventually; evaluate Keysight OpenTAP as potential option in addition to TestExec/TestLib/TestPlan.  https://opentap.io/.
// - Briefly evaluated previously; time for reevaluation.
// NOTE:  Recommend using Microsoft's Visual Studio Code to develop/debug Tests based closed source/proprietary projects:
//        - Visual Studio Code is a co$t free, open-source Integrated Development Environment entirely suitable for textual C# development, like Tests.
//          - That is, it's excellent for non-GUI (WinForms/WPF/WinUI) C# development.
//          - VS Code is free for both private & commercial use:
//            - https://code.visualstudio.com/docs/supporting/FAQ
//            - https://code.visualstudio.com/license
// NOTE:  Recommend using Microsoft's Visual Studio Community Edition to develop/debug open sourced TestExecutive:
//        - https://github.com/Amphenol-Borisch-Technologies/TestExecutive/blob/master/LICENSE.txt
//        - "An unlimited number of users within an organization can use Visual Studio Community for the following scenarios:
//           in a classroom learning environment, for academic research, or for contributing to open source projects."
//        - Tests based projects are closed source/proprietary, which are disqualified from using VS Studio Community Edition.
//          - https://visualstudio.microsoft.com/vs/community/
//          - https://visualstudio.microsoft.com/license-terms/vs2022-ga-community/
// NOTE:  - VS Studio Community Edition is more preferable for GUI C# development than VS Code.
//          - If not developing GUI code (WinForms/WPF/WinUI), then VS Code is entirely sufficient & potentially preferable.
// TODO:  Eventually; refactor TestExec to Microsoft's C# Coding Conventions, https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions.
// NOTE:  For public methods, will deviate by using PascalCasing for parameters.  Will use recommended camelCasing for internal & private method parameters.
//        - Prefer named arguments for public methods be Capitalized/PascalCased, not uncapitalized/camelCased.
//        - Invoking public methods with named arguments is a superb, self-documenting coding technique, improved by PascalCasing.
// TODO:  Eventually; add documentation per https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments.
// TODO:  Eventually; update to .Net 8.0 & C# 12.0 instead of .Net FrameWork 4.8 & C# 7.3.

namespace ABT.Test.TestExecutive.TestExec {
    /// <remarks>
    ///  <b>References:</b>
    /// <item>
    ///  <description><a href="https://github.com/Amphenol-Borisch-Technologies/TestExecutive">TestExecutive</a></description>
    ///  <description><a href="https://github.com/Amphenol-Borisch-Technologies/TestPlan">TestPlan</a></description>
    ///  </item>
    ///  </remarks>
    /// <summary>
    /// NOTE:  Test Developer is responsible for ensuring Methods can be both safely &amp; correctly called in sequence defined in TestPlanDefinition.xml:
    /// <para>
    ///        - That is, if Methods execute sequentially as (M1, M2, M3, M4, M5), Test Developer is responsible for ensuring all equipment is
    ///          configured safely &amp; correctly between each Method.
    ///          - If:
    ///            - M1 is unpowered Shorts &amp; Opens measurements.
    ///            - M2 is powered voltage measurements.
    ///            - M3 begins with unpowered operator cable connections/disconnections for In-System Programming.
    ///          - Then Test Developer must ensure necessary equipment state transitions are implemented so test operator isn't
    ///            plugging/unplugging a powered UUT in T03.
    /// </para>
    /// </summary>
    /// 
    /// <summary>
    /// NOTE:  Two types of TestExec cancellations possible, each having two sub-types resulting in 4 altogether:
    /// <para>
    /// A) Spontaneous Operator Initiated Cancellations:
    ///      1)  Operator Proactive:
    ///          - Microsoft's recommended CancellationTokenSource technique, permitting Operator to proactively
    ///            cancel currently executing Method.
    ///          - Requires Test project implementation by the Test Developer, but is initiated by Operator, so categorized as such.
    ///          - Implementation necessary if the *currently* executing Method must be cancellable during execution by the Operator.
    ///          - https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads
    ///          - https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-cancellation
    ///          - https://learn.microsoft.com/en-us/dotnet/standard/threading/canceling-threads-cooperatively
    ///      2)  Operator Reactive:
    ///          - TestExec's already implemented, always available &amp; default reactive "Cancel before next Test" technique,
    ///            which simply invokes CTS_Cancel.Cancel().
    ///          - CTS_Cancel.IsCancellationRequested is checked at the end of TestExec.MethodsRun()'s foreach loop.
    ///            - If true, TestExec.MethodsRun()'s foreach loop is broken, causing reactive cancellation.
    ///            prior to the next Method's execution.
    ///          - Note: This doesn't proactively cancel the *currently* executing Method, which runs to completion.
    /// B) PrePlanned Developer Programmed Cancellations:
    ///      3)  TestExec Developer initiated cancellations:
    ///          - Any Tests project's Method can initiate a cancellation programmatically by simply throwing an OperationCanceledException:
    ///          - Permits immediate cancellation if specific condition(s) occur in a Method; perhaps to prevent UUT or equipment damage,
    ///            or simply because futher execution is pointless.
    ///          - Simply throw an OperationCanceledException if the specific condition(s) occcur.
    ///      4)  TestPlanDefinition.xml's CancelNotPassed:
    ///          - TestPlanDefinition.xml's Method elements have Boolean "CancelNotPassed" fields:
    ///          - If the current Test.MethodRun() has CancelNotPassed=true and it's resulting EvaluateResultMethod() doesn't return EVENTS.PASS,
    ///            TestExec.MethodsRun() will break/exit, stopping further testing.
    ///		    - Do not pass Go, do not collect $200, go directly to TestExec.MethodsPostRun().
    ///
    /// NOTE:  The Operator Proactive &amp; TestExec Developer initiated cancellations both occur while the currently executing Tests.MethodRun() conpletes, via 
    ///        thrown OperationCanceledException.
    /// NOTE:  The Operator Reactive &amp; TestPlanDefinition.xml's CancelNotPassed cancellations both occur after the currently executing Tests.MethodRun() completes, via checks
    ///        inside the Exec.MethodsRun() loop.
    /// </para>
    /// </summary>
    public abstract partial class TestExec : Form {
        public static System.Timers.Timer StatusTimer = new System.Timers.Timer(15000);
        public readonly String TestPlanFolder;
        private readonly Miscellaneous.SerialNumberDialog _serialNumberDialog = null;

        protected TestExec(Icon icon, String testPlanFolder) {
            InitializeComponent();
            Icon = icon; // NOTE:  https://stackoverflow.com/questions/40933304/how-to-create-an-icon-for-visual-studio-with-just-mspaint-and-visual-studio
            TestPlanDefinitionXML_Path = testPlanFolder + @"\" + TestPlanDefinitionBase + xml;
            if (TestPlanDefinitionValidator.ValidDefinition(TestPlanDefinitionXML_Path)) testPlanDefinition = Serializing.DeserializeFromFile<TestPlanDefinition>(xmlFile: $"{TestPlanDefinitionXML_Path}");
            else throw new ArgumentException($"Invalid XML '{TestPlanDefinitionXML_Path}'; doesn't comply with XSD '{TestPlanDefinitionXSD_Path}'.");
            InstrumentDrivers = GetInstrumentDriversTestPlanDefinition();
            UserName = GetUserPrincipal();

            TSMI_UUT_TestData.Enabled = testPlanDefinition.SerialNumberEntry.IsEnabled();
            if (TSMI_UUT_TestData.Enabled) {
                if (!(testExecDefinition.TestData.Item is TextFiles) && !(testExecDefinition.TestData.Item is SQL_DB)) throw new ArgumentException($"Unknown {nameof(TestPlanDefinition)}.{nameof(TestData)}.{nameof(TestData.Item)} '{nameof(testExecDefinition.TestData.Item)}'.");
                TSMI_UUT_TestDataP_DriveTDR_Folder.Enabled = (testExecDefinition.TestData.Item is TextFiles);
                TSMI_UUT_TestDataSQL_ReportingAndQuerying.Enabled = (testExecDefinition.TestData.Item is SQL_DB);

                if (RegexInvalid(testPlanDefinition.SerialNumberEntry.RegularEx)) throw new ArgumentException($"Invalid {nameof(SerialNumberEntry.RegularEx)} '{testPlanDefinition.SerialNumberEntry.RegularEx}' in file '{TestPlanDefinitionXML_Path}'.");
                if (testPlanDefinition.SerialNumberEntry.EntryType is SerialNumberEntryType.Barcode) _serialNumberDialog = new Miscellaneous.SerialNumberDialog(testPlanDefinition.SerialNumberEntry.RegularEx, testPlanDefinition.SerialNumberEntry.Format, testExecDefinition.BarcodeReader.ID);
            }

            StatusTimer.Elapsed += StatusTimeUpdate;
            StatusTimer.AutoReset = true;
            CTS_Cancel = new CancellationTokenSource();
            CT_Cancel = CTS_Cancel.Token;
            CTS_EmergencyStop = new CancellationTokenSource();
            CT_EmergencyStop = CTS_EmergencyStop.Token;
        }

        #region Form Miscellaneous
        private String GetUserPrincipal() {
            String UserName;
            try { UserName = UserPrincipal.Current.DisplayName; } catch {
                // NOTE:  UserPrincipal.Current.DisplayName requires a connected/active Domain session for Active Directory PCs.
                UserName = InputForm.Show(Title: "Enter your full name for test data.", SystemIcons.Question);
            }
            UserName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(UserName);
            return UserName;
        }

        private void Form_Closing(Object sender, FormClosingEventArgs e) {
            SystemReset();
            _serialNumberDialog?.Close();
        }

        private void Form_Shown(Object sender, EventArgs e) { ButtonSelect_Click(sender, e); }

        private void FormModeReset() {
            TextTest.Text = String.Empty;
            TextTest.BackColor = Color.White;
            TextTest.Refresh();
            rtfResults.Text = String.Empty;
            StatusTimeUpdate(null, null);
            StatusStatisticsUpdate(null, null);
            StatusCustomWrite(String.Empty, SystemColors.ControlLight);
            StatusModeUpdate(MODES.Resetting);
        }

        private void FormModeRun() {
            ButtonCancelReset(enabled: true);
            ButtonEmergencyStopReset(enabled: true);
            TSMI_TestPlan.Enabled = false;
            ButtonSelect.Enabled = false;
            ButtonRunReset(enabled: false);
            TSMI_System_SelfTests.Enabled = false;
            TSMI_UUT_Statistics.Enabled = false;
            StatusModeUpdate(MODES.Running);
        }

        private void FormModeWait() {
            ButtonCancelReset(enabled: false);
            ButtonEmergencyStopReset(enabled: false);
            TSMI_TestPlan.Enabled = true;
            ButtonSelect.Enabled = true;
            ButtonRunReset(enabled: testSequence != null);
            TSMI_System_SelfTests.Enabled = true;
            TSMI_UUT_Statistics.Enabled = true;
            StatusModeUpdate(MODES.Waiting);
        }

        public virtual void SystemReset() {
            if (testPlanDefinition.TestSpace.Simulate) return;
            IPowerSuppliesOutputsOff();
            IInstrumentsResetClear();
            IRelaysOpenAll();
        }

        public virtual void IInstrumentsResetClear() {
            if (testPlanDefinition.TestSpace.Simulate) return;
            foreach (KeyValuePair<String, Object> kvp in InstrumentDrivers) if (kvp.Value is IInstrument iInstrument) iInstrument.ResetClear();
        }

        public virtual void IPowerSuppliesOutputsOff() {
            if (testPlanDefinition.TestSpace.Simulate) return;
            foreach (KeyValuePair<String, Object> kvp in InstrumentDrivers) if (kvp.Value is IPowerSupply iIPowerSupply) iIPowerSupply.OutputsOff();
        }

        public virtual void IRelaysOpenAll() {
            if (testPlanDefinition.TestSpace.Simulate) return;
            foreach (KeyValuePair<String, Object> kvp in InstrumentDrivers) if (kvp.Value is IRelay iRelay) iRelay.OpenAll();
        }
        #endregion Form Miscellaneous

        #region Form Command Buttons
        private void ButtonCancel_Enter(Object sender, EventArgs e) {
            TSMI_About_TestPlan.Select(); // Prevents ButtonCanel or ButtonEmergencyStop from having focus, so if a MessageBox loses focus while testing and operator presses keyboard Enter key, won't cancel or Emergency Stop.
        }

        private void ButtonCancel_Clicked(Object sender, EventArgs e) {
            ButtonCancelReset(enabled: false);
            StatusModeUpdate(MODES.Cancelling);
            CTS_Cancel.Cancel();
        }

        private void ButtonCancelReset(Boolean enabled) {
            if (enabled) {
                ButtonCancel.UseVisualStyleBackColor = false;
                ButtonCancel.BackColor = Color.Yellow;
            } else {
                ButtonCancel.BackColor = SystemColors.Control;
                ButtonCancel.UseVisualStyleBackColor = true;
            }
            if (CTS_Cancel.IsCancellationRequested) {
                CTS_Cancel.Dispose();
                CTS_Cancel = new CancellationTokenSource();
                CT_Cancel = CTS_Cancel.Token;
            }
            ButtonCancel.Enabled = enabled;
        }

        private void ButtonEmergencyStop_Enter(Object sender, EventArgs e) {
            TSMI_About_TestPlan.Select(); // Prevents ButtonCanel or ButtonEmergencyStop from having focus, so if a MessageBox loses focus while testing and operator presses keyboard Enter key, won't cancel or Emergency Stop.
        }

        private void ButtonEmergencyStop_Clicked(Object sender, EventArgs e) {
            ButtonEmergencyStop.Enabled = false;
            ButtonCancelReset(enabled: false);
            StatusModeUpdate(MODES.Emergency_Stopping);
            SystemReset();
            CTS_EmergencyStop.Cancel();
        }

        private void ButtonEmergencyStopReset(Boolean enabled) {
            if (CT_EmergencyStop.IsCancellationRequested) {
                CTS_EmergencyStop.Dispose();
                CTS_EmergencyStop = new CancellationTokenSource();
                CT_EmergencyStop = CTS_EmergencyStop.Token;
            }
            ButtonEmergencyStop.Enabled = enabled;
        }

        private void ButtonSelect_Click(Object sender, EventArgs e) {
            testSequence = TestSelect.Get();
            base.Text = $"{testSequence.UUT.Number}, {testSequence.UUT.Description}, {(testSequence.IsOperation ? testSequence.TestOperation.NamespaceTrunk : testSequence.TestOperation.TestGroups[0].Classname)}";
            StatusTimer.Start();
            FormModeReset();
            FormModeWait();
        }

        private async void ButtonRun_Clicked(Object sender, EventArgs e) {
            TSMI_About_TestPlan.Select(); // Prevents ButtonCanel or ButtonEmergencyStop from having focus, so if a MessageBox loses focus while testing and operator presses keyboard Enter key, won't cancel or Emergency Stop.
            if (testPlanDefinition.SerialNumberEntry.IsEnabled()) {
                String serialNumber;
                if (testPlanDefinition.SerialNumberEntry.EntryType is SerialNumberEntryType.Barcode) {
                    _serialNumberDialog.Set(testSequence.SerialNumber);
                    serialNumber = _serialNumberDialog.ShowDialog(this).Equals(DialogResult.OK) ? _serialNumberDialog.Get() : String.Empty;
                    _serialNumberDialog.Hide();
                } else {
                    serialNumber = Interaction.InputBox(
                        Prompt: $"Please enter Serial Number in below format:{Environment.NewLine}{Environment.NewLine}" +
                        $"{testPlanDefinition.SerialNumberEntry.Format}",
                        Title: "Enter Serial Number", DefaultResponse: testSequence.SerialNumber).Trim().ToUpper();
                    serialNumber = Regex.IsMatch(serialNumber, testPlanDefinition.SerialNumberEntry.RegularEx) ? serialNumber : String.Empty;
                }
                if (String.Equals(serialNumber, String.Empty)) return;
                testSequence.SerialNumber = serialNumber;
            }

            FormModeReset();
            FormModeRun();
            MethodsPreRun();
            await MethodsRun();
            MethodsPostRun();
            FormModeWait();
        }

        private void ButtonRunReset(Boolean enabled) {
            if (enabled) {
                ButtonRun.UseVisualStyleBackColor = false;
                ButtonRun.BackColor = Color.Green;
            } else {
                ButtonRun.BackColor = SystemColors.Control;
                ButtonRun.UseVisualStyleBackColor = true;
            }
            ButtonRun.Enabled = enabled;
        }
        #endregion Form Command Buttons

        #region Form Tool Strip Menu Items
        private void TSMI_TestPlan_Choose_Click(Object sender, EventArgs e) {
            // NOTE: Canonical method to load/unload DLLs in .Net Framework is AppDomain.
            // - But AppDomains require marshalling across process boundaries, as AppDomains are their own separate processes.
            // - Further, AppDomains aren't supported in .Net, just .Net Framework.
            // - .Net instead provides AssemblyLoadContext which would be perfect for TestExec...but isn't available in .Net Framework.
            // - Thus this compromise.
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = testExecDefinition.TestPlansFolder;
                openFileDialog.Filter = "TestPlan Files|*.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    TestChooser.TestChooser.Launch(openFileDialog.FileName, Process.GetCurrentProcess().Id);
                    Application.Exit();
                }
            }
        }
        private void TSMI_TestPlan_SaveResults_Click(Object sender, EventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "Save Test Results",
                Filter = "Rich Text Format|*.rtf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = $"{testSequence.UUT.Number}_{testSequence.TestOperation.NamespaceTrunk}_{testSequence.SerialNumber}",
                DefaultExt = "rtf",
                CreatePrompt = false,
                OverwritePrompt = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) rtfResults.SaveFile(saveFileDialog.FileName);
        }
        private void TSMI_TestPlan_Exit_Click(Object sender, EventArgs e) { Application.Exit(); }

        private void TSMI_System_ColorCode_Click(Object sender, EventArgs e) {
            CustomMessageBox customMessageBox = new CustomMessageBox {
                Icon = SystemIcons.Information,
                Text = "Event Color Codes"
            };
            RichTextBox richTextBox = (RichTextBox)customMessageBox.Controls["richTextBox"];
            richTextBox.Font = new Font(richTextBox.Font.FontFamily, 12);
            richTextBox.Text = String.Empty;
            foreach (EVENTS Event in Enum.GetValues(typeof(EVENTS))) richTextBox.Text += $"{nameof(EVENTS)}.{Event}{Environment.NewLine}{Environment.NewLine}";

            foreach (EVENTS Event in Enum.GetValues(typeof(EVENTS))) {
                String s = $"{nameof(EVENTS)}.{Event}";
                richTextBox.SelectionStart = richTextBox.Find(s, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                richTextBox.SelectionLength = s.Length;
                if (EventColors.ContainsKey(Event)) richTextBox.SelectionBackColor = EventColors[Event];
                else throw new InvalidOperationException($"Test Engineering needs to update '{nameof(EventColors)}', associating '{nameof(EVENTS)}.{Event}' with a color!");
            }
            customMessageBox.ShowDialog();
        }
        private void TSMI_System_SelfTestsInstruments_Click(Object sender, EventArgs e) {
            UseWaitCursor = true;
            Boolean passed = true;
            foreach (KeyValuePair<String, Object> kvp in InstrumentDrivers) passed &= ((IInstrument)kvp.Value).SelfTests() is SELF_TEST_RESULTS.PASS;
            if (passed) _ = MessageBox.Show(this, "SCPI VISA Instrument Self-Tests all passed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UseWaitCursor = false;
        }
        private void TSMI_System_ManualsBarcodeScanner_Click(Object sender, EventArgs e) { OpenFolder(testExecDefinition.BarcodeReader.Folder); }
        private void TSMI_System_ManualsInstruments_Click(Object sender, EventArgs e) { OpenFolder(testExecDefinition.InstrumentsTestExec.Folder); }

        private void TSMI_UUT_eDocs_Click(Object sender, EventArgs e) {
            foreach (Documentation documentation in testPlanDefinition.UUT.Documentation) OpenFolder(documentation.Folder);
        }
        private void TSMI_UUT_Manuals_Click(Object sender, EventArgs e) {
            foreach (Documentation documentation in testPlanDefinition.Development.Documentation) OpenFolder(documentation.Folder);
        }
        private void TSMI_UUT_StatisticsDisplay_Click(Object sender, EventArgs e) {
            Form statistics = new Miscellaneous.MessageBoxMonoSpaced(
                Title: $"{testSequence.UUT.Number}, {testSequence.TestOperation.NamespaceTrunk}, {testPlanDefinition.TestSpace.StatusTime()}",
                Text: testPlanDefinition.TestSpace.StatisticsDisplay(),
                Link: String.Empty
            );
            _ = statistics.ShowDialog();
        }
        private void TSMI_UUT_StatisticsReset_Click(Object sender, EventArgs e) {
            testPlanDefinition.TestSpace.Statistics = new Statistics();
            StatusTimeUpdate(null, null);
            StatusStatisticsUpdate(null, null);
        }
        private void TSMI_UUT_TestData_P_DriveTDR_Folder_Click(Object sender, EventArgs e) {
            Debug.Assert(testExecDefinition.TestData.Item is TextFiles);
            OpenFolder($"{((TextFiles)testExecDefinition.TestData.Item).Folder}\\{testPlanDefinition.UUT.Number}\\{testSequence.TestOperation.NamespaceTrunk}");
        }
        private void TSMI_UUT_TestDataSQL_ReportingAndQuerying_Click(Object sender, EventArgs e) {
            Debug.Assert(testExecDefinition.TestData.Item is SQL_DB);
            OpenApp(testExecDefinition.Apps.Microsoft.SQLServerManagementStudio);
        }
        private void TSMI_About_TestExec_Click(Object sender, EventArgs e) {
            Development development = Serializing.DeserializeFromFile<Development>(TestExecDefinitionXML_Path);
            ShowAbout(Assembly.GetExecutingAssembly(), development, isTestPlan: false);
        }
        private void TSMI_About_TestPlan_Click(Object sender, EventArgs e) {
            ShowAbout(Assembly.GetEntryAssembly(), testPlanDefinition.Development, isTestPlan: true);
        }
        private void ShowAbout(Assembly assembly, Development development, Boolean isTestPlan) {
            StringBuilder stringBuilder = new StringBuilder();
            const Int32 PR = 16;
            stringBuilder.AppendLine($"{nameof(Assembly)}:");
            stringBuilder.AppendLine($"\t{nameof(Name)}".PadRight(PR) + $": {assembly.GetName().Name}");
            stringBuilder.AppendLine($"\t{nameof(Version)}".PadRight(PR) + $": {assembly.GetName().Version}");
            AssemblyCopyrightAttribute assemblyCopyrightAttribute = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute));
            String copyRight = assemblyCopyrightAttribute is null ? "© Amphenol Borisch Technologies" : assemblyCopyrightAttribute.Copyright;
            stringBuilder.AppendLine($"\t{copyRight}{Environment.NewLine}{Environment.NewLine}");

            stringBuilder.AppendLine($"{nameof(Repository)}(s):");
            foreach (Repository repository in development.Repository) stringBuilder.AppendLine($"\t{nameof(Repository.URL)}".PadRight(PR) + $": {repository.URL}");
            stringBuilder.AppendLine($"{Environment.NewLine}");

            stringBuilder.AppendLine($"{nameof(Development)}:");
            stringBuilder.AppendLine($"\t{nameof(Development.Released)}".PadRight(PR) + $": {development.Released:d}");
            foreach (Developer developer in development.Developer) stringBuilder.AppendLine($"\t{nameof(Development)}".PadRight(PR) + $": {developer.Name}, {developer.Language}.");
            foreach (Documentation documentation in development.Documentation) stringBuilder.AppendLine($"\t{nameof(Documentation)}".PadRight(PR) + $": {ConvertWindowsPathToUrl(documentation.Folder)}");

            CustomMessageBox.Show(Title: $"About {(isTestPlan ? "TestPlan " : String.Empty)}{assembly.GetName().Name}", Message: stringBuilder.ToString());
        }
        #endregion Form Tool Strip Menu Items

        #region Form Status Strip
        private void StatusTimeUpdate(Object source, ElapsedEventArgs e) { StatusTimeLabel.Text = testPlanDefinition.TestSpace.StatusTime(); }

        private void StatusStatisticsUpdate(Object source, ElapsedEventArgs e) { StatusStatisticsLabel.Text = testPlanDefinition.TestSpace.StatisticsStatus(); }

        private enum MODES { Resetting, Running, Cancelling, Emergency_Stopping, Waiting };

        private readonly Dictionary<MODES, Color> ModeColors = new Dictionary<MODES, Color>() {
            { MODES.Resetting, EventColors[EVENTS.UNSET] },
            { MODES.Running, EventColors[EVENTS.PASS] },
            { MODES.Cancelling, EventColors[EVENTS.CANCEL] },
            { MODES.Emergency_Stopping, EventColors[EVENTS.EMERGENCY_STOP] },
            { MODES.Waiting, Color.Black }
        };

        private void StatusModeUpdate(MODES mode) {
            StatusModeLabel.Text = Enum.GetName(typeof(MODES), mode);
            StatusModeLabel.ForeColor = ModeColors[mode];
        }

        public void StatusCustomWrite(String Message, Color ForeColor) {
            _ = Invoke((Action)(() => StatusCustomLabel.Text = Message));
            _ = Invoke((Action)(() => StatusCustomLabel.ForeColor = ForeColor));
        }
        #endregion Form Status Strip

        #region Form Logging
        private static readonly String MessageTestEvent = "Test Event";
        private static readonly String MessageUUT_Event = (Spaces2 + MessageTestEvent).PadRight(PaddingRight) + ": ";

        #region Public Methods
        public void LogAppend(String message) {
            Int32 startFind = rtfResults.TextLength;

            if (rtfResults.InvokeRequired) {
                rtfResults.BeginInvoke((MethodInvoker)(() => LogAppend(message)));
            } else {
                rtfResults.AppendText(message + Environment.NewLine);
            }

            Int32 selectionStart;
            foreach (EVENTS Event in Enum.GetValues(typeof(EVENTS))) {
                if (message.Contains(Event.ToString())) {
                    selectionStart = rtfResults.Find(Event.ToString(), startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                    rtfResults.SelectionStart = selectionStart;
                    rtfResults.SelectionLength = Event.ToString().Length;
                    rtfResults.SelectionBackColor = EventColors[Event];
                }
            }
        }
        public void LogError(String logMessage) { LogAppend(logMessage); }
        #endregion Public Methods

        #region Private Methods
        private void LogMethod(Method method) {
            LogSetBackColor(startFind: 0, findString: method.Name, backColor: EventColors[method.Event]);
            if (method.Event is EVENTS.PASS) return;
            StringBuilder stringBuilder = new StringBuilder(((IFormat)method).Format());
            stringBuilder.AppendLine(FormatMessage(MessageTestEvent, method.Event.ToString()));
            stringBuilder.Append($"{Spaces2}{method.Log}");
            Int32 startFind = rtfResults.TextLength;
            LogAppend(stringBuilder.ToString());
            LogSetBackColors(startFind, findString: EVENTS.FAIL.ToString(), backColor: EventColors[EVENTS.FAIL]);
            LogSetBackColors(startFind, findString: EVENTS.PASS.ToString(), backColor: EventColors[EVENTS.PASS]);
        }

        private void LogStart() {
            LogAppend($"{nameof(UUT)}:");
            LogAppend($"{MessageUUT_Event}");
            LogAppend($"{Spaces2}{nameof(TestSequence.SerialNumber)}".PadRight(PaddingRight) + $": {testSequence.SerialNumber}");
            LogAppend($"{Spaces2}{nameof(UUT.Number)}".PadRight(PaddingRight) + $": {testSequence.UUT.Number}");
            LogAppend($"{Spaces2}{nameof(UUT.Revision)}".PadRight(PaddingRight) + $": {testSequence.UUT.Revision}");
            LogAppend($"{Spaces2}{nameof(UUT.Description)}".PadRight(PaddingRight) + $": {testSequence.UUT.Description}");
            LogAppend($"{Spaces2}{nameof(UUT.Category)}".PadRight(PaddingRight) + $": {testSequence.UUT.Category}");
            LogAppend($"{Spaces2}{nameof(UUT.Customer)}".PadRight(PaddingRight) + $": {testSequence.UUT.Customer.Name}\n");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{nameof(TestGroup.Methods)}:");
            String SPACING = Spaces2 + Spaces2; // Embedded tabs in strings (\t) seem to cause method ReplaceText() issues.
            foreach (TestGroup testGroup in testSequence.TestOperation.TestGroups) {
                stringBuilder.AppendLine($"{Spaces2}{testGroup.Classname}, {testGroup.Description}");
                foreach (Method method in testGroup.Methods) stringBuilder.AppendLine($"{SPACING}{method.Name}".PadRight(PaddingRight + SPACING.Length) + $": {method.Description}");
            }
            LogAppend(stringBuilder.ToString());
        }

        private void LogStop() {
            LogReplaceString(startFind: 0, findString: $"{MessageUUT_Event}", replacementString: $"{MessageUUT_Event}{testSequence.Event}");
            LogSetBackColor(startFind: 0, findString: testSequence.Event.ToString(), backColor: EventColors[testSequence.Event]);
            if (testSequence.IsOperation && testPlanDefinition.SerialNumberEntry.EntryType != SerialNumberEntryType.None) {
                if (testExecDefinition.TestData.Item is TextFiles) LogStopTextFiles();
                else if (testExecDefinition.TestData.Item is SQL_DB) LogStopSQL_DB();
                else throw new ArgumentException($"Unknown {nameof(TestData)} item '{testExecDefinition.TestData.Item}'.");
            }
        }

        private void LogReplaceString(Int32 startFind, String findString, String replacementString) {
            Int32 selectionStart = rtfResults.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
            rtfResults.SelectionStart = selectionStart;
            rtfResults.SelectionLength = findString.Length;
            rtfResults.SelectedText = replacementString;
        }

        private void LogReplaceStrings(Int32 startFind, String findString, String replacementString) {
            Int32 selectionStart;

            while (startFind < rtfResults.TextLength) {
                selectionStart = rtfResults.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                if (selectionStart == -1) break;
                rtfResults.SelectionStart = selectionStart;
                rtfResults.SelectionLength = findString.Length;
                rtfResults.SelectedText = replacementString;
                startFind = selectionStart + findString.Length;
            }
        }

        private void LogSetBackColor(Int32 startFind, String findString, Color backColor) {
            Int32 selectionStart = rtfResults.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
            rtfResults.SelectionStart = selectionStart;
            rtfResults.SelectionLength = findString.Length;
            rtfResults.SelectionBackColor = backColor;
        }

        private void LogSetBackColors(Int32 startFind, String findString, Color backColor) {
            Int32 selectionStart;
            while (startFind < rtfResults.TextLength) {
                selectionStart = rtfResults.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                if (selectionStart == -1) break;
                rtfResults.SelectionStart = selectionStart;
                rtfResults.SelectionLength = findString.Length;
                rtfResults.SelectionBackColor = backColor;
                startFind = selectionStart + findString.Length;
            }
        }

        private void LogStopTextFiles() {
            String xmlFolder = $"{((TextFiles)testExecDefinition.TestData.Item).Folder}\\{testPlanDefinition.UUT.Number}\\{testSequence.TestOperation.NamespaceTrunk}";
            String xmlBaseName = $"{testSequence.UUT.Number}_{testSequence.SerialNumber}_{testSequence.TestOperation.NamespaceTrunk}";
            String[] xmlFileNames;
            try {
                xmlFileNames = Directory.GetFiles(xmlFolder, $"{xmlBaseName}_*{xml}", SearchOption.TopDirectoryOnly);
            } catch {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Logging error:");
                stringBuilder.AppendLine($"   Folder         : '{xmlFolder}'.");
                stringBuilder.AppendLine($"   Base File Name : '{xmlBaseName}_*{xml}'.");
                MessageBox.Show(stringBuilder.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                throw;
            }
            Int32 maxNumber = 0; String s;
            foreach (String xmlFileName in xmlFileNames) {
                s = xmlFileName;
                foreach (EVENTS Event in Enum.GetValues(typeof(EVENTS))) s = s.Replace(Event.ToString(), String.Empty);
                s = s.Replace($"{xmlFolder}\\{xmlBaseName}", String.Empty);
                s = s.Replace(xml, String.Empty);
                s = s.Replace("_", String.Empty);

                if (Int32.Parse(s) > maxNumber) maxNumber = Int32.Parse(s);
            }

            using (FileStream fileStream = new FileStream($"{xmlFolder}\\{xmlBaseName}_{++maxNumber}_{testSequence.Event}{xml}", FileMode.CreateNew)) {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, new UTF8Encoding(true))) {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestSequence), LogGetOverrides());
                    xmlSerializer.Serialize(xmlTextWriter, testSequence);
                }
            }
        }

        private void LogStopSQL_DB() {
            using (StringWriter stringWriter = new StringWriter()) {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Encoding = new UTF8Encoding(true), Indent = true })) {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestSequence), LogGetOverrides());
                    xmlSerializer.Serialize(xmlWriter, testSequence);
                    xmlWriter.Flush();

                    using (SqlConnection sqlConnection = new SqlConnection(((SQL_DB)testExecDefinition.TestData.Item).ConnectionString)) {
                        using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO Sequences (Sequence) VALUES (@XML)", sqlConnection)) {
                            sqlCommand.Parameters.AddWithValue("@XML", stringWriter.ToString());
                            sqlConnection.Open();
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private XmlAttributeOverrides LogGetOverrides() {
            XmlAttributes xmlAttributes;
            XmlAttributeOverrides xmlAttributeOverrides = new XmlAttributeOverrides();
            xmlAttributes = new XmlAttributes { XmlIgnore = true };
            xmlAttributeOverrides.Add(typeof(UUT), nameof(UUT.Documentation), xmlAttributes);
            xmlAttributes = new XmlAttributes { XmlIgnore = true };
            xmlAttributeOverrides.Add(typeof(TestOperation), nameof(TestOperation.ProductionTest), xmlAttributes);
            xmlAttributes = new XmlAttributes { XmlIgnore = true };
            xmlAttributeOverrides.Add(typeof(Method), nameof(Method.CancelNotPassed), xmlAttributes);
            xmlAttributes = new XmlAttributes { XmlIgnore = true };
            xmlAttributeOverrides.Add(typeof(TestGroup), nameof(TestGroup.CancelNotPassed), xmlAttributes);
            xmlAttributes = new XmlAttributes { XmlIgnore = true };
            xmlAttributeOverrides.Add(typeof(TestGroup), nameof(TestGroup.Independent), xmlAttributes);
            return xmlAttributeOverrides;
        }
        #endregion Private
        #endregion Form Logging

        #region Form Methods
        private void MethodsPreRun() {
            testSequence.PreRun();
            TestIndices.Nullify();
            LogStart();
            SystemReset();
        }

        private async Task MethodsRun() {
            TestIndices.TestOperation = testSequence.TestOperation;
            foreach (TestGroup testGroup in testSequence.TestOperation.TestGroups) {
                TestIndices.TestGroup = testGroup;
                foreach (Method method in testGroup.Methods) {
                    TestIndices.Method = method;
                    try {
                        method.Value = await Task.Run(() => MethodRun(method));
                        method.Event = ((IEvaluate)method).Evaluate();
                        method.LogString = method.Log.ToString(); // NOTE:  XmlSerializer doesn't support [OnSerializing] attribute, so have to explicitly invoke LogConvert().
                        if (CT_EmergencyStop.IsCancellationRequested || CT_Cancel.IsCancellationRequested) {
                            SystemReset();
                            return;
                        }
                    } catch (Exception exception) {
                        SystemReset();
                        if (exception.ToString().Contains(typeof(OperationCanceledException).Name)) {
                            method.Event = EVENTS.CANCEL;// NOTE:  May be altered to EVENTS.EMERGENCY_STOP in finally block.
                            while (!(exception is OperationCanceledException) && (exception.InnerException != null)) exception = exception.InnerException; // No fluff, just stuff.
                            _ = method.Log.AppendLine($"{Environment.NewLine}{typeof(OperationCanceledException).Name}:{Environment.NewLine}{exception.Message}");
                        }
                        if (!CT_EmergencyStop.IsCancellationRequested && !CT_Cancel.IsCancellationRequested) {
                            method.Event = EVENTS.ERROR;
                            _ = method.Log.AppendLine($"{Environment.NewLine}{exception}");
                            ErrorMessage(exception.ToString());
                        }
                        return;
                    } finally {
                        // NOTE:  Normally executes, regardless if catchable Exception occurs or returned out of try/catch blocks.
                        // Exceptional exceptions are exempted; https://stackoverflow.com/questions/345091/will-code-in-a-finally-statement-fire-if-i-return-a-value-in-a-try-block.
                        if (CT_EmergencyStop.IsCancellationRequested) method.Event = EVENTS.EMERGENCY_STOP;
                        else if (CT_Cancel.IsCancellationRequested) method.Event = EVENTS.CANCEL;
                        // NOTE:  Both CT_Cancel.IsCancellationRequested & CT_EmergencyStop.IsCancellationRequested could be true; prioritize CT_EmergencyStop.
                        LogMethod(method);
                    }
                    if (method.Event != EVENTS.PASS && method.CancelNotPassed) return;
                }
                if (GroupEvaluate(testGroup) != EVENTS.PASS && testGroup.CancelNotPassed) return;
            }
        }

        protected abstract Task<String> MethodRun(Method method);

        private void MethodsPostRun() {
            SystemReset();
            testSequence.PostRun(OperationEvaluate(testSequence.TestOperation));
            TextTest.Text = testSequence.Event.ToString();
            TextTest.BackColor = EventColors[testSequence.Event];
            testPlanDefinition.TestSpace.Statistics.Update(testSequence.Event);
            StatusStatisticsUpdate(null, null);
            LogStop();
        }

        private Boolean EventSet(Int32 aggregatedEvents, EVENTS events) { return ((aggregatedEvents & (Int32)events) == (Int32)events); }

        private EVENTS GroupEvaluate(TestGroup testGroup) {
            Int32 groupEvents = 0;
            foreach (Method method in testGroup.Methods) groupEvents |= (Int32)method.Event;
            foreach (EVENTS events in Enum.GetValues(typeof(EVENTS))) if (EventSet(groupEvents, events)) return events;

            throw new NotImplementedException(($"{nameof(testGroup.Classname)}: '{testGroup.Classname}', {nameof(testGroup.Description)} '{testGroup.Description}' doesn't contain valid {nameof(EVENTS)}."));
        }

        private EVENTS OperationEvaluate(TestOperation testOperation) {
            Int32 operationEvents = 0;
            foreach (TestGroup testGroup in testOperation.TestGroups) operationEvents |= (Int32)GroupEvaluate(testGroup);
            foreach (EVENTS events in Enum.GetValues(typeof(EVENTS))) if (EventSet(operationEvents, events)) return events;

            throw new NotImplementedException(($"{nameof(testOperation.NamespaceTrunk)}: '{testOperation.NamespaceTrunk}', {nameof(testOperation.Description)} '{testOperation.Description}' doesn't contain valid {nameof(EVENTS)}."));
        }
        #endregion Form Methods
    }
}
