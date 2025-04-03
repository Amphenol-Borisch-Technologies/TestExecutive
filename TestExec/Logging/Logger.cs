#undef VERBOSE
using ABT.Test.TestLib;
using ABT.Test.TestLib.Configuration;
using Serilog; // Install Serilog via NuGet Package Manager.  Site is https://serilog.net/.
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using static ABT.Test.TestLib.Data;

namespace ABT.Test.TestExec.Logging {
    public static class Logger {
        public const String LOGGER_TEMPLATE = "{Message}{NewLine}";
        private const String MESSAGE_TEST_EVENT = "Test Event";
        private static readonly String MESSAGE_UUT_EVENT = (SPACES_2 + MESSAGE_TEST_EVENT).PadRight(PAD_RIGHT) + ": ";

        #region Public Methods
        public static void LogError(String logMessage) { Log.Error(logMessage); }

        public static void LogMessageAppend(String Message) { Log.Information(Message); }

        public static void LogMessageAppendLine(String Message) { Log.Information($"{Message}{Environment.NewLine}"); }

        public static void LogMethod(ref RichTextBox rtfResults, Method method) {
            SetBackColor(ref rtfResults, 0, method.Name, EventColors[method.Event]);
            if (method.Event is EVENTS.PASS) return;
            StringBuilder stringBuilder = new StringBuilder(((IFormat)method).Format());
            stringBuilder.AppendLine(FormatMessage(MESSAGE_TEST_EVENT, method.Event.ToString()));
            stringBuilder.Append($"{SPACES_2}{method.Log}");
            Int32 startFind = rtfResults.TextLength;
            Log.Information(stringBuilder.ToString());
            SetBackColors(ref rtfResults, startFind, EVENTS.FAIL.ToString(), EventColors[EVENTS.FAIL]);
            SetBackColors(ref rtfResults, startFind, EVENTS.PASS.ToString(), EventColors[EVENTS.PASS]);
        }

        public static void Start(ref RichTextBox rtfResults) {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Sink(new RichTextBoxSink(richTextBox: ref rtfResults, outputTemplate: LOGGER_TEMPLATE))
                .CreateLogger();

            Log.Information($"{nameof(UUT)}:");
            Log.Information($"{MESSAGE_UUT_EVENT}");
            Log.Information($"{SPACES_2}{nameof(TestSequence.SerialNumber)}".PadRight(PAD_RIGHT) + $": {testSequence.SerialNumber}");
            Log.Information($"{SPACES_2}{nameof(UUT.Number)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Number}");
            Log.Information($"{SPACES_2}{nameof(UUT.Revision)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Revision}");
            Log.Information($"{SPACES_2}{nameof(UUT.Description)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Description}");
            Log.Information($"{SPACES_2}{nameof(UUT.Category)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Category}");
            Log.Information($"{SPACES_2}{nameof(UUT.Customer)}".PadRight(PAD_RIGHT) + $": {testSequence.UUT.Customer.Name}\n");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{nameof(TestGroup.Methods)}:");
            const String SPACING = SPACES_2 + SPACES_2; // Embedded tabs in strings (\t) seem to cause method ReplaceText() issues.
            foreach (TestGroup testGroup in testSequence.TestOperation.TestGroups) {
                stringBuilder.AppendLine($"{SPACES_2}{testGroup.Classname}, {testGroup.Description}");
                foreach (Method method in testGroup.Methods) stringBuilder.AppendLine($"{SPACING}{method.Name}".PadRight(PAD_RIGHT + SPACING.Length) + $": {method.Description}");
            }
            Log.Information(stringBuilder.ToString());
        }

        public static void Stop(ref RichTextBox rtfResults) {
            ReplaceString(ref rtfResults, 0, $"{MESSAGE_UUT_EVENT}", $"{MESSAGE_UUT_EVENT}{testSequence.Event}");
            SetBackColor(ref rtfResults, 0, testSequence.Event.ToString(), EventColors[testSequence.Event]);
            Log.CloseAndFlush();
            if (testSequence.IsOperation && testPlanDefinition.SerialNumberEntry.EntryType != SerialNumberEntryType.None) {
                if (testExecDefinition.TestData.Item is TextFiles) StopTextFiles();
                else if (testExecDefinition.TestData.Item is SQL_DB) StopSQL_DB();
                else throw new ArgumentException($"Unknown {nameof(TestData)} item '{testExecDefinition.TestData.Item}'.");
            }
        }
        #endregion Public Methods

        #region Private Methods
        private static void ReplaceString(ref RichTextBox richTextBox, Int32 startFind, String findString, String replacementString) {
            Int32 selectionStart = richTextBox.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
            if (selectionStart == -1) Log.Error($"Rich Text '{findString}' not found after character '{startFind}', cannot replace with '{replacementString}'.");
            else {
                richTextBox.SelectionStart = selectionStart;
                richTextBox.SelectionLength = findString.Length;
                richTextBox.SelectedText = replacementString;
            }
        }

        private static void ReplaceStrings(ref RichTextBox richTextBox, Int32 startFind, String findString, String replacementString) {
            Int32 selectionStart;

            while (startFind < richTextBox.TextLength) {
                selectionStart = richTextBox.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                if (selectionStart == -1) break;
                richTextBox.SelectionStart = selectionStart;
                richTextBox.SelectionLength = findString.Length;
                richTextBox.SelectedText = replacementString;
                startFind = selectionStart + findString.Length;
            }
        }

        private static void SetBackColor(ref RichTextBox richTextBox, Int32 startFind, String findString, Color backColor) {
            Int32 selectionStart = richTextBox.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
            if (selectionStart == -1) Log.Error($"Rich Text '{findString}' not found after character '{startFind}', cannot highlight with '{backColor.Name}'.");
            else {
                richTextBox.SelectionStart = selectionStart;
                richTextBox.SelectionLength = findString.Length;
                richTextBox.SelectionBackColor = backColor;
            }
        }

        private static void SetBackColors(ref RichTextBox richTextBox, Int32 startFind, String findString, Color backColor) {
            Int32 selectionStart;
            while (startFind < richTextBox.TextLength) {
                selectionStart = richTextBox.Find(findString, startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                if (selectionStart == -1) break;
                richTextBox.SelectionStart = selectionStart;
                richTextBox.SelectionLength = findString.Length;
                richTextBox.SelectionBackColor = backColor;
                startFind = selectionStart + findString.Length;
            }
        }

        private static void StopTextFiles() {
            const String _xml = ".xml";
            String xmlFolder = $"{((TextFiles)testExecDefinition.TestData.Item).Folder}\\{testPlanDefinition.UUT.Number}\\{testSequence.TestOperation.NamespaceTrunk}";
            String xmlBaseName = $"{testSequence.UUT.Number}_{testSequence.SerialNumber}_{testSequence.TestOperation.NamespaceTrunk}";
            String[] xmlFileNames = Directory.GetFiles(xmlFolder, $"{xmlBaseName}_*{_xml}", SearchOption.TopDirectoryOnly);
            // NOTE:  Will fail if invalid path.  Don't catch resulting Exception though; this has to be fixed in TestPlanDefinitionXML.
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