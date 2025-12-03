using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tektronix.Tkdpo2k3k4k.Interop;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_IVI_COM : IInstrument, IDiagnostics, IDisposable {
        public String Address { get; }
        public String Detail { get; }
        public Tkdpo2k3k4kClass Tkdpo2k3k4kClass { get; }
        public enum BUSES { B1, B2 }
        public enum CHANNELS { CH1, CH2 }
        public enum DRIVES_USB { E, F }
        public INSTRUMENT_TYPES InstrumentType { get; }
        public enum SETUPS { SETUP1 = 1, SETUP2 = 2, SETUP3 = 3, SETUP4 = 4, SETUP5 = 5, SETUP6 = 6, SETUP7 = 7, SETUP8 = 8, SETUP9 = 9, SETUP10 = 10 }
        public readonly static String ValidCharactersFile = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=+-!@#$%^&()[]{}~‘’,";
        public readonly static String ValidCharactersLabel = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=≠+-±!@#$%^&*()[]{}<>/~‘’\"\\|:,.?µ∞∆°Ωσ";
        private Boolean _disposed = false;

        public void ResetClear() { Tkdpo2k3k4kClass.Reset(); }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 TestResult = 0;
            String TestMessage = String.Empty;
            try {
                Tkdpo2k3k4kClass.UtilityEx.SelfTest(ref TestResult, ref TestMessage);
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)TestResult; // Tkdpo2k3k4kClass returns 0 for passed, 1 for fail.
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_3014 = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                // TODO: Eventually; add verification measurements of the MSO-3014 mixed signal oscilloscope using external instrumentation.
            }
            return result_3014;
        }

        public MSO_3014_IVI_COM(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            Tkdpo2k3k4kClass = new Tkdpo2k3k4kClass();
            Tkdpo2k3k4kClass.Initialize(ResourceName: Address, IdQuery: false, Reset: false, OptionString: String.Empty);
        }

        public void OperationCompleteQuery(String scpiCommand) {
            Tkdpo2k3k4kClass.WriteString("*OPC?");
            if (Tkdpo2k3k4kClass.ReadString().Trim().Trim('"') != "1") throw new InvalidOperationException($"{Detail}, Address '{Address}' didn't complete SCPI command '{scpiCommand}'!");
        }
        public void EventTableEnable(BUSES Bus) {
            switch (Bus) {
                case BUSES.B1:
                    Tkdpo2k3k4kClass.WriteString(":FPAnel:PRESS B1;:*WAI");
                    break;
                case BUSES.B2:
                    Tkdpo2k3k4kClass.WriteString(":FPAnel:PRESS B2;:*WAI");
                    break;
                default:
                    throw new NotImplementedException(NotImplementedMessageEnum<BUSES>(Enum.GetName(typeof(BUSES), Bus)));
            }
            Tkdpo2k3k4kClass.WriteString(":FPAnel:PRESS BMENU7;:*WAI");
            Tkdpo2k3k4kClass.WriteString(":FPAnel:PRESS RMENU1;:*WAI");
            Tkdpo2k3k4kClass.WriteString(":FPAnel:PRESS MENUOff;:*WAI");
            Tkdpo2k3k4kClass.WriteString(":FPAnel:PRESS MENUOff;:*WAI");
        }

        public Boolean SetupExists(SETUPS Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            Tkdpo2k3k4kClass.WriteString($":{Setup}:LABEL?");
            return Tkdpo2k3k4kClass.ReadString().Trim().Trim('"').Equals(LabelString);
        }

        public void SetupLoad(SETUPS Setup, String LabelString) {
            if (!SetupExists(Setup, LabelString)) throw new ArgumentException($"MSO-3014 {Setup} labled '{LabelString}' non-existent!");
            Tkdpo2k3k4kClass.WriteString($":RECAll:SETUp {(Int32)Setup}");
            OperationCompleteQuery($":RECAll:SETUp {(Int32)Setup}");
        }

        public void SetupLoad(String SetupFilePath) {
            if (!File.Exists(SetupFilePath)) throw new FileNotFoundException($"MSO-3014 Setup file not found at path '{SetupFilePath}'!");
            foreach (String mso_3014_SCPI_Command in File.ReadLines(SetupFilePath)) {
                Tkdpo2k3k4kClass.WriteString(mso_3014_SCPI_Command);
            }
        }

        public void SetupSave(SETUPS Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            Tkdpo2k3k4kClass.WriteString($":{Setup}:LABEL \"{LabelString}\"");
            Tkdpo2k3k4kClass.WriteString($":{Setup}:LABEL?");
            String labelRead = Tkdpo2k3k4kClass.ReadString().Trim().Trim('"');
            if (!labelRead.Equals(LabelString)) throw new ArgumentException($"MSO-3014 {Setup} not labeled correctly!{Environment.NewLine}  Should be '{LabelString}'.{Environment.NewLine}  Is '{labelRead}'.");
        }

        public Boolean ValidFileCharacters(String FileString) { return FileString.All(new HashSet<Char>(ValidCharactersFile.ToCharArray()).Contains); }

        public Boolean ValidFilel(String FileString) { return ((FileString.Length < 125) && ValidFileCharacters(FileString)); }

        public Boolean ValidLabelCharacters(String LabelString) { return LabelString.All(new HashSet<Char>(ValidCharactersLabel.ToCharArray()).Contains); }

        public Boolean ValidLabel(String LabelString) { return ((LabelString.Length < 30) && ValidLabelCharacters(LabelString)); }

        private String InvalidLabelMessage(String LabelString) { return $"Invalid MSO-3014 Setup label '{LabelString}'!{Environment.NewLine}  Label cannot exceed 30 characters in length and can only contain characters in set \"{ValidCharactersLabel}\"."; }

        ~MSO_3014_IVI_COM() { Dispose(false); }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing) {
            if (!_disposed) {
                if (disposing) {
                    // Free managed resources here
                    Tkdpo2k3k4kClass.Close();
                }
                // Free unmanaged resources here (if any).
                _disposed = true;
            }
        }
    }
}