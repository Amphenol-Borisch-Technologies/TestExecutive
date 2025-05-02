using ABT.Test.TestExecutive.TestLib;
using ABT.Test.TestExecutive.TestLib.Configuration;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestExec.Logging {
    public static class Logger {
        private static readonly String MESSAGE_TEST_EVENT = "Test Event";
        private static readonly String MESSAGE_UUT_EVENT = (SPACES_2 + MESSAGE_TEST_EVENT).PadRight(PAD_RIGHT) + ": ";

        #region Public Methods
        public static void LogError(RichTextBox rtfResults, String logMessage) { Append(rtfResults, logMessage); }

        public static void LogMethod(RichTextBox rtfResults, Method method) {
            SetBackColor(rtfResults, 0, method.Name, EventColors[method.Event]);
            if (method.Event is EVENTS.PASS) return;
            StringBuilder stringBuilder = new StringBuilder(((IFormat)method).Format());
            stringBuilder.AppendLine(FormatMessage(MESSAGE_TEST_EVENT, method.Event.ToString()));
            stringBuilder.Append($"{SPACES_2}{method.Log}");
            Int32 startFind = rtfResults.TextLength;
            Append(rtfResults, stringBuilder.ToString());
            SetBackColors(rtfResults, startFind, EVENTS.FAIL.ToString(), EventColors[EVENTS.FAIL]);
            SetBackColors(rtfResults, startFind, EVENTS.PASS.ToString(), EventColors[EVENTS.PASS]);
        }

        public static void Start(RichTextBox rtfResults) {
            Append(rtfResults, $"{nameof(UUT)}:");
            Append(rtfResults, $"{MESSAGE_UUT_EVENT}");
            Append(rtfResults, $"{SPACES_2}{nameof(TestSequence.SerialNumber)}".PadRight(PAD_RIGHT) + $": {testSequence.SerialNumber}");
            Append(rtfResults, $"{SPACES_2}{nameof(UUT.Number)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Number}");
            Append(rtfResults, $"{SPACES_2}{nameof(UUT.Revision)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Revision}");
            Append(rtfResults, $"{SPACES_2}{nameof(UUT.Description)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Description}");
            Append(rtfResults, $"{SPACES_2}{nameof(UUT.Category)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Category}");
            Append(rtfResults, $"{SPACES_2}{nameof(UUT.Customer)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Customer.Name}\n");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{nameof(TestGroup.Methods)}:");
            String SPACING = SPACES_2 + SPACES_2; // Embedded tabs in strings (\t) seem to cause method ReplaceText() issues.
            foreach (TestGroup testGroup in testSequence.TestOperation.TestGroups) {
                stringBuilder.AppendLine($"{SPACES_2}{testGroup.Classname}, {testGroup.Description}");
                foreach (Method method in testGroup.Methods) stringBuilder.AppendLine($"{SPACING}{method.Name}".PadRight(PAD_RIGHT + SPACING.Length) + $": {method.Description}");
            }
            Append(rtfResults, stringBuilder.ToString());
        }

        public static void Stop(RichTextBox rtfResults) {
            ReplaceString(rtfResults, 0, $"{MESSAGE_UUT_EVENT}", $"{MESSAGE_UUT_EVENT}{testSequence.Event}");
            SetBackColor(rtfResults, 0, testSequence.Event.ToString(), EventColors[testSequence.Event]);
            if (testSequence.IsOperation && testPlanDefinition.SerialNumberEntry.EntryType != SerialNumberEntryType.None) {
                if (testExecDefinition.TestData.Item is TextFiles) StopTextFiles();
                else if (testExecDefinition.TestData.Item is SQL_DB) StopSQL_DB();
                else throw new ArgumentException($"Unknown {nameof(TestData)} item '{testExecDefinition.TestData.Item}'.");
            }
        }

        public static void Append(RichTextBox rtfResults, String message) {
            Int32 startFind = rtfResults.TextLength;

            if (rtfResults.InvokeRequired) {
                rtfResults.BeginInvoke((MethodInvoker)(() => Append(rtfResults, message)));
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
        #endregion Public Methods

        #region Private Methods
        private static void ReplaceString(RichTextBox rtfResults, Int32 startFind, String findString, String replacementString) {
            Int32 selectionStart = rtfResults.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
            rtfResults.SelectionStart = selectionStart;
            rtfResults.SelectionLength = findString.Length;
            rtfResults.SelectedText = replacementString;
        }

        private static void ReplaceStrings(RichTextBox rtfResults, Int32 startFind, String findString, String replacementString) {
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

        private static void SetBackColor(RichTextBox rtfResults, Int32 startFind, String findString, Color backColor) {
            Int32 selectionStart = rtfResults.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
            rtfResults.SelectionStart = selectionStart;
            rtfResults.SelectionLength = findString.Length;
            rtfResults.SelectionBackColor = backColor;
        }

        private static void SetBackColors(RichTextBox rtfResults, Int32 startFind, String findString, Color backColor) {
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

        private static void StopTextFiles() {
            const String _xml = ".xml";
            String xmlFolder = $"{((TextFiles)testExecDefinition.TestData.Item).Folder}\\{testPlanDefinition.UUT.Number}\\{testSequence.TestOperation.NamespaceTrunk}";
            String xmlBaseName = $"{testSequence.UUT.Number}_{testSequence.SerialNumber}_{testSequence.TestOperation.NamespaceTrunk}";
            String[] xmlFileNames;
            try {
                xmlFileNames = Directory.GetFiles(xmlFolder, $"{xmlBaseName}_*{_xml}", SearchOption.TopDirectoryOnly);
            } catch {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Logging error:");
                stringBuilder.AppendLine($"   Folder         : '{xmlFolder}'.");
                stringBuilder.AppendLine($"   Base File Name : '{xmlBaseName}_*{_xml}'.");
                MessageBox.Show(stringBuilder.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                throw;
            }
            Int32 maxNumber = 0; String s;
            foreach (String xmlFileName in xmlFileNames) {
                s = xmlFileName;
                foreach (EVENTS Event in Enum.GetValues(typeof(EVENTS))) s = s.Replace(Event.ToString(), String.Empty);
                s = s.Replace($"{xmlFolder}\\{xmlBaseName}", String.Empty);
                s = s.Replace(_xml, String.Empty);
                s = s.Replace("_", String.Empty);

                if (Int32.Parse(s) > maxNumber) maxNumber = Int32.Parse(s);
            }

            using (FileStream fileStream = new FileStream($"{xmlFolder}\\{xmlBaseName}_{++maxNumber}_{testSequence.Event}{_xml}", FileMode.CreateNew)) {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, new UTF8Encoding(true))) {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestSequence), GetOverrides());
                    xmlSerializer.Serialize(xmlTextWriter, testSequence);
                }
            }
        }

        private static void StopSQL_DB() {
            using (StringWriter stringWriter = new StringWriter()) {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Encoding = new UTF8Encoding(true), Indent = true })) {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestSequence), GetOverrides());
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

        private static XmlAttributeOverrides GetOverrides() {
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
    }
}