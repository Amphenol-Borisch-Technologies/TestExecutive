using ABT.Test.TestLib;
using Serilog.Core; // Install Serilog via NuGet Package Manager.  Site is https://serilog.net/.
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.IO;
using System.Windows.Forms;

namespace ABT.Test.TestExec.Logging {
    public class RichTextBoxSink : ILogEventSink {
        private readonly RichTextBox richTextBox;
        private readonly ITextFormatter formatter;

        public RichTextBoxSink(ref RichTextBox richTextBox, String outputTemplate = Logger.LOGGER_TEMPLATE) {
            this.richTextBox = richTextBox;
            formatter = new MessageTemplateTextFormatter(outputTemplate);
        }

        public void Emit(LogEvent logEvent) {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            StringWriter stringWriter = new StringWriter();
            formatter.Format(logEvent, stringWriter);
            Int32 startFind = richTextBox.TextLength;
            String logMessage = stringWriter.ToString();
            richTextBox.InvokeIfRequired(() => richTextBox.AppendText(logMessage));

            Int32 selectionStart;
            foreach (EVENTS Event in Enum.GetValues(typeof(EVENTS))) {
                if (logMessage.Contains(Event.ToString())) {
                    selectionStart = richTextBox.Find(Event.ToString(), startFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                    richTextBox.SelectionStart = selectionStart;
                    richTextBox.SelectionLength = Event.ToString().Length;
                    richTextBox.SelectionBackColor = Data.EventColors[Event];
                }
            }
        }
    }

    public static class WinFormsControlExtensions {
        public static void InvokeIfRequired(this Control c, MethodInvoker action) {
            if (c.InvokeRequired) c.Invoke(action);
            else action();
        }
    }
}
